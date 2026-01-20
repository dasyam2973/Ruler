using Ruler.Core.Helpers;

namespace Ruler.Core;

public sealed class SequenceRule<T1, T2> : Rule<(T1, T2)>
{
    private readonly Rule<T1> _first;
    private readonly Rule<T2> _second;

    public SequenceRule(Rule<T1> first, Rule<T2> second)
    {
        _first = first;
        _second = second;
    }

    protected override RulerResult<(T1, T2)> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<(T1, T2)>.Exhausted();

        return Sequence.Apply(cursor, _first, _second);
    }
}
public sealed class SequenceRule<T1, T2, T3> : Rule<(T1, T2, T3)>
{
    private readonly Rule<T1> _first;
    private readonly Rule<T2> _second;
    private readonly Rule<T3> _third;

    public SequenceRule(Rule<T1> first, Rule<T2> second, Rule<T3> third)
    {
        _first = first;
        _second = second;
        _third = third;
    }

    protected override RulerResult<(T1, T2, T3)> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<(T1, T2, T3)>.Exhausted();

        return Sequence.Apply(cursor, _first, _second, _third);
    }
}
