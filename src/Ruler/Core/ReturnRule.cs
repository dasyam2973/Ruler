namespace Ruler.Core;

public sealed class ReturnRule<T, U> : Rule<U>
{
    private readonly Rule<T> _inner;
    private readonly U _value;

    public ReturnRule(Rule<T> inner, U value)
    {
        _inner = inner;
        _value = value;
    }

    protected override RulerResult<U> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<U>.Exhausted();

        RulerResult<T> innerRes = _inner.FirstMatch(cursor);
        if (innerRes.IsSuccess)
        {
            return RulerResult<U>.Success(innerRes.Cursor, _value);
        }
        return RulerResult<U>.Failure(innerRes.ErrorInfo);
    }
}
