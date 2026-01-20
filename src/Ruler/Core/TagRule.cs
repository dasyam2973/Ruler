namespace Ruler.Core;

public sealed class TagRule<T> : Rule<T>
{
    private readonly Rule<T> _inner;
    private readonly string _tag;

    public TagRule(Rule<T> inner, string tag)
    {
        _inner = inner;
        _tag = tag;
    }

    protected override RulerResult<T> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<T>.Exhausted();

        RulerResult<T> innerRes = _inner.FirstMatch(cursor);
        if (innerRes.IsFailure && string.IsNullOrEmpty(innerRes.ErrorInfo.Tag))
            return RulerResult<T>.Failure(innerRes.ErrorInfo.WithTag(_tag));
        return innerRes;
    }
}
