namespace Ruler.Core;

public sealed class IgnoreRule<T> : Rule<Void>
{
    private readonly Rule<T> _inner;

    public IgnoreRule(Rule<T> inner)
    {
        _inner = inner;
    }

    protected override RulerResult<Void> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<Void>.Exhausted();

        RulerResult<T> innerRes = _inner.FirstMatch(cursor);
        if (innerRes.IsSuccess)
        {
            return RulerResult<Void>.Success(innerRes.Cursor, Void.Default);
        }
        return RulerResult<Void>.Failure(innerRes.ErrorInfo);
    }
}
