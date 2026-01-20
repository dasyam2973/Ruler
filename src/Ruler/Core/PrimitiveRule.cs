namespace Ruler.Core;

public sealed class PrimitiveRule<T> : Rule<T>
{
    private readonly Parser<T> _parser;

    public PrimitiveRule(Parser<T> parser)
    {
        _parser = parser;
    }

    protected override RulerResult<T> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<T>.Exhausted();

        return _parser(cursor);
    }
}
