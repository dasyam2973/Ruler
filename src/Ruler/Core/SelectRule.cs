namespace Ruler.Core;

public sealed class SelectRule<T, TResult> : Rule<TResult>
{
    private readonly Rule<T> _inner;
    private readonly Func<T, TResult> _selector;

    public SelectRule(Rule<T> inner, Func<T, TResult> selector)
    {
        _inner = inner;
        _selector = selector;
    }

    protected override RulerResult<TResult> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<TResult>.Exhausted();

        RulerResult<T> innerRes = _inner.FirstMatch(cursor);
        if (innerRes.IsSuccess)
        {
            return RulerResult<TResult>.Success(innerRes.Cursor, _selector(innerRes.Value));
        }
        return RulerResult<TResult>.Failure(innerRes.ErrorInfo);
    }
}
