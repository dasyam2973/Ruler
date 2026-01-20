namespace Ruler.Core;

public sealed class WhereRule<T> : Rule<T>
{
    private readonly Rule<T> _inner;
    private readonly Func<T, bool> _predicate;
    private readonly string _expected;

    public WhereRule(Rule<T> inner, Func<T, bool> predicate, string expected = "")
    {
        _inner = inner;
        _predicate = predicate;
        _expected = expected;
    }

    public override bool IsAmbiguous => true;

    protected override RulerResult<T> Apply(TextCursor cursor, int step)
    {
        RulerResult<T> result = _inner.ApplyInternal(cursor, step);
        if (result.IsSuccess && !_predicate(result.Value))
        {
            return RulerResult<T>.Failure(string.IsNullOrEmpty(_expected)
                ? ErrorInfo.RawMessage(cursor, ErrorMessages.PredicateFailure)
                : ErrorInfo.Expected(cursor, _expected));
        }
        return result;
    }
}
