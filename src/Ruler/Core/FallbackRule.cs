namespace Ruler.Core;

public sealed class FallbackRule<T> : Rule<T>
{
    private readonly Rule<T> _primary;
    private readonly Rule<T> _fallback;

    public FallbackRule(Rule<T> primary, Rule<T> fallback)
    {
        _primary = primary;
        _fallback = fallback;
    }

    protected override RulerResult<T> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<T>.Exhausted();

        RulerResult<T> primaryRes = _primary.FirstMatch(cursor);
        if (primaryRes.IsSuccess)
        {
            return primaryRes;
        }
        return _fallback.FirstMatch(cursor);
    }
}
