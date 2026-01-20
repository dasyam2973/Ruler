namespace Ruler.Core;

public sealed class OptionalRule<T> : Rule<T?>
{
    private readonly Rule<T> _inner;

    public OptionalRule(Rule<T> inner)
    {
        _inner = inner;
    }

    protected override RulerResult<T?> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<T?>.Exhausted();

        RulerResult<T> innerRes = _inner.FirstMatch(cursor);
        if (innerRes.IsSuccess)
        {
            return RulerResult<T?>.Success(innerRes.Cursor, innerRes.Value);
        }
        return RulerResult<T?>.Success(cursor, default);
    }
}
