using Ruler.Core;

namespace Ruler;

public static partial class RuleExtensions
{
    public static Rule<T?> Optional<T>(this Rule<T> rule)
        => new OptionalRule<T>(rule);

    public static Rule<Void> Ignore<T>(this Rule<T> rule)
        => new IgnoreRule<T>(rule);

    public static Rule<U> Return<T, U>(this Rule<T> rule, U value)
        => new ReturnRule<T, U>(rule, value);

    public static Rule<T> Or<T>(this Rule<T> left, Rule<T> right)
        => new ChoiceRule<T>(left, right);

    public static Rule<TRight> Then<TLeft, TRight>(this Rule<TLeft> left, Func<TLeft, Rule<TRight>> selector)
        => new ThenRule<TLeft, TRight>(left, selector);

    public static Rule<TRight> Then<TRight>(this Rule<Void> left, Rule<TRight> right)
        => new ThenRule<Void, TRight>(left, _ => right);

    public static Rule<TRight> IgnoreThen<TLeft, TRight>(this Rule<TLeft> left, Rule<TRight> right)
        => new ThenRule<TLeft, TRight>(left, _ => right);

    public static Rule<T> Fallback<T>(this Rule<T> primary, Rule<T> fallback)
        => new FallbackRule<T>(primary, fallback);

    public static Rule<T[]> Many0<T>(this Rule<T> element)
        => new RepeatRule<T>(element);

    public static Rule<T[]> Many1<T>(this Rule<T> element)
        => new RepeatRule<T>(element, min: 1);

    public static Rule<T[]> Repeat<T>(this Rule<T> element, int count)
        => new RepeatRule<T>(element, count, count);

    public static Rule<T[]> SeparatedBy0<T, U>(this Rule<T> element, Rule<U> separator)
        => new SeparatedByRule<T, U>(element, separator);

    public static Rule<T[]> SeparatedBy1<T, U>(this Rule<T> element, Rule<U> separator)
        => new SeparatedByRule<T, U>(element, separator, min: 1);

    public static Rule<T> Tag<T>(this Rule<T> rule, string tag)
        => new TagRule<T>(rule, tag);

    public static Rule<T> Between<T, TLeft, TRight>(this Rule<T> rule, Rule<TLeft> left, Rule<TRight> right)
        => Rules.Rules.Sequence(left, rule, right).Select(tuple => tuple.Item2);

    public static Rule<TValue> Chain<TValue, TOperator>(this Rule<TValue> operand, Rule<TOperator> op, Func<TValue, TOperator, TValue, TValue> combiner)
        => new ChainRule<TValue, TOperator>(operand, op, combiner);

    public static Rule<T> Trim<T>(this Rule<T> rule)
        => rule.Between(Rules.Rules.WhiteSpaces, Rules.Rules.WhiteSpaces);

    public static Rule<T> EndOfInput<T>(this Rule<T> rule)
        => rule.Then(v => Rules.Rules.EndOfInput.Select(_ => v));


    public static Rule<TResult> Select<T, TResult>(this Rule<T> rule, Func<T, TResult> selector)
        => new SelectRule<T, TResult>(rule, selector);

    public static Rule<TResult> SelectMany<T, U, TResult>(this Rule<T> rule, Func<T, Rule<U>> selector, Func<T, U, TResult> resultSelector)
        => new SelectManyRule<T, U, TResult>(rule, selector, resultSelector);

    public static Rule<T> Where<T>(this Rule<T> rule, Func<T, bool> predicate)
        => new WhereRule<T>(rule, predicate);
}
