namespace Ruler.Core;

public sealed class ChoiceRule<T> : Rule<T>
{
    private readonly Rule<T>[] _rules;

    public ChoiceRule(params Rule<T>[] rules)
    {
        _rules = rules;
    }

    public override bool IsAmbiguous => true;

    protected override RulerResult<T> Apply(TextCursor cursor, int step)
    {
        if (step >= 0 && step < _rules.Length)
        {
            return _rules[step].FirstMatch(cursor);
        }
        return RulerResult<T>.Exhausted();
    }
}
