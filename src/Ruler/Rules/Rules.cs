using Ruler.Core;

namespace Ruler.Rules;

public static partial class Rules
{
    public static readonly Rule<Void> EndOfInput = new PrimitiveRule<Void>(static (cursor) =>
    {
        if (!cursor.IsEndOfInput)
            return RulerResult<Void>.Failure(ErrorInfo.Expected(cursor, ErrorMessages.EndOfInput));

        return RulerResult<Void>.Success(cursor, Void.Default);
    });

    public static Rule<T> Lazy<T>(Func<Rule<T>> ruleFactory)
        => new LazyRule<T>(ruleFactory);

    public static Rule<T> Recursive<T>(Func<Rule<T>, Rule<T>> ruleFactory)
    {
        Rule<T>? rule = null;
        return rule = ruleFactory(Lazy(() => rule!));
    }

    public static Rule<T> Choice<T>(params Rule<T>[] rules)
        => new ChoiceRule<T>(rules);

    public static Rule<(T1, T2)> Sequence<T1, T2>(Rule<T1> first, Rule<T2> second)
        => new SequenceRule<T1, T2>(first, second);

    public static Rule<(T1, T2, T3)> Sequence<T1, T2, T3>(Rule<T1> first, Rule<T2> second, Rule<T3> third)
        => new SequenceRule<T1, T2, T3>(first, second, third);

    public static Rule<string> Token(string s)
        => String(s).Trim();

    public static Rule<T> Token<T>(string s, T value)
        => String(s).Trim().Return(value);

    public static Rule<T> TokenIgnoreCase<T>(string s, T value)
        => IgnoreCase(s).Trim().Return(value);
}
