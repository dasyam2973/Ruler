using Ruler.Core;

namespace Ruler;

public abstract class Rule<T>
{
    /// <summary>
    /// 모호한 Rule인지에 대한 결과를 반환합니다.
    /// </summary>
    /// <returns>
    /// 해당 Rule이 하나의 결과만을 가질 수 있으면 <see langword="true"/>를 반환합니다.
    /// 여러 결과를 가질 수 있으면, <see langword="false"/>를 반환합니다.
    /// </returns>
    public virtual bool IsAmbiguous => false;

    /// <summary>
    /// 텍스트의 위치 정보(<paramref name="cursor"/>)와 탐색 단계(<paramref name="step"/>)를 기반으로 Rule을 적용한 결과를 반환합니다.
    /// </summary>
    /// <param name="step">
    /// Rule 탐색 시 시도할 현재 단계입니다. 동일한 시작 위치에서 여러 결과가 있을 경우 이 값을 통해 다음 대안을 가져옵니다.
    /// </param>
    /// <returns>
    /// 매칭 성공 여부, 결과 값, 그리고 다음 파싱 위치를 포함하는 <see cref="RulerResult{T}"/> 객체입니다.
    /// </returns>
    protected abstract RulerResult<T> Apply(TextCursor cursor, int step);

    public RulerResult<T> Apply(string s)
    {
        return FirstMatch(new(s));
    }

    public T ApplyOrThrow(string s)
    {
        RulerResult<T> result = Apply(s);
        if (result.IsSuccess)
            return result.Value!;
        throw new ParseException(result.ErrorInfo);
    }

    internal RulerResult<T> ApplyInternal(TextCursor cursor, int step)
    {
        return Apply(cursor, step);
    }

    protected static RulerResult<TChild> ApplyChild<TChild>(Rule<TChild> childRule, TextCursor cursor, int step)
    {
        return childRule.Apply(cursor, step);
    }

    public RulerResult<T> FirstMatch(TextCursor cursor)
    {
        if (!IsAmbiguous)
            return Apply(cursor, 0);

        ErrorRefiner refiner = new();
        for (int i = 0; ; i++)
        {
            RulerResult<T> ruleResult = Apply(cursor, i);
            if (ruleResult.IsSuccess)
            {
                return ruleResult;
            }
            else if (ruleResult.EndOfSteps) break;
            else if (ruleResult.IsFailure) refiner.Update(ruleResult.ErrorInfo);
        }
        return RulerResult<T>.Failure(refiner.Get());
    }

    public Rule<U> As<U>()
    {
        return this.Select(value =>
        {
            if (value is U casted)
                return casted;
            throw new InvalidCastException($"Cannot cast {typeof(T).Name} to {typeof(U).Name}.");
        });
    }
}
