namespace Ruler.Core;

public sealed class ThenRule<TLeft, TRight> : Rule<TRight>
{
    private readonly Rule<TLeft> _left;
    private readonly Func<TLeft, Rule<TRight>> _selector;

    public ThenRule(Rule<TLeft> left, Func<TLeft, Rule<TRight>> selector)
    {
        _left = left;
        _selector = selector;
    }

    protected override RulerResult<TRight> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<TRight>.Exhausted();

        ErrorRefiner refiner = new();
        for (int i = 0; ; i++)
        {
            RulerResult<TLeft> leftRes = _left.ApplyInternal(cursor, i);
            if (leftRes.IsSuccess)
            {
                Rule<TRight> right = _selector(leftRes.Value);
                for (int j = 0; ; j++)
                {
                    RulerResult<TRight> rightRes = right.ApplyInternal(leftRes.Cursor, j);
                    if (rightRes.IsSuccess)
                    {
                        return RulerResult<TRight>.Success(rightRes.Cursor, rightRes.Value);
                    }
                    else if (rightRes.EndOfSteps) break;
                    else if (rightRes.IsFailure) refiner.Update(rightRes.ErrorInfo);
                }
            }
            else if (leftRes.EndOfSteps) break;
            else if (leftRes.IsFailure) refiner.Update(leftRes.ErrorInfo);
        }
        return RulerResult<TRight>.Failure(refiner.Get());
    }
}
