namespace Ruler.Core;

public sealed class LazyRule<T> : Rule<T>
{
    private readonly Func<Rule<T>> _ruleFactory;
    private Rule<T>? _rule;

    public LazyRule(Func<Rule<T>> ruleFactory)
    {
        _ruleFactory = ruleFactory;
    }

    private Rule<T> Rule => _rule ??= _ruleFactory();

    protected override RulerResult<T> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<T>.Exhausted();

        return Rule.FirstMatch(cursor);
    }
}
