namespace Ruler.Core;

public sealed class SelectManyRule<T, U, TResult> : Rule<TResult>
{
    private readonly Rule<T> _left;
    private readonly Func<T, Rule<U>> _selector;
    private readonly Func<T, U, TResult> _resultSelector;

    public SelectManyRule(Rule<T> left, Func<T, Rule<U>> selector, Func<T, U, TResult> resultSelector)
    {
        _left = left;
        _selector = selector;
        _resultSelector = resultSelector;
    }

    protected override RulerResult<TResult> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<TResult>.Exhausted();

        ErrorRefiner refiner = new();
        for (int i = 0; ; i++)
        {
            RulerResult<T> leftRes = _left.ApplyInternal(cursor, i);
            if (leftRes.IsSuccess)
            {
                Rule<U> right = _selector(leftRes.Value);
                for (int j = 0; ; j++)
                {
                    RulerResult<U> rightRes = right.ApplyInternal(leftRes.Cursor, j);
                    if (rightRes.IsSuccess)
                    {
                        return RulerResult<TResult>.Success(rightRes.Cursor, _resultSelector(leftRes.Value, rightRes.Value));
                    }
                    else if (rightRes.EndOfSteps) break;
                    else if (rightRes.IsFailure) refiner.Update(rightRes.ErrorInfo);
                }
            }
            else if (leftRes.EndOfSteps) break;
            else if (leftRes.IsFailure) refiner.Update(leftRes.ErrorInfo);
        }
        return RulerResult<TResult>.Failure(refiner.Get());
    }
}
