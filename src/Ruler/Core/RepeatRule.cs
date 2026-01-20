namespace Ruler.Core;

public sealed class RepeatRule<T> : Rule<T[]>
{
    private readonly Rule<T> _element;
    private readonly int _min;
    private readonly int _max;

    public RepeatRule(Rule<T> element, int min = 0, int max = int.MaxValue)
    {
        _element = element;
        _min = min;
        _max = max;
    }

    protected override RulerResult<T[]> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<T[]>.Exhausted();

        TextCursor start = cursor;
        List<T> values = new();

        while (true)
        {
            bool foundAny = false;
            for (int i = 0; ; i++)
            {
                RulerResult<T> elementRes = _element.ApplyInternal(cursor, i);
                if (elementRes.IsSuccess && elementRes.Cursor.Position > cursor.Position)
                {
                    foundAny = true;
                    values.Add(elementRes.Value);
                    cursor = elementRes.Cursor;
                    break;
                }
                else if (elementRes.EndOfSteps) break;
            }
            if (!foundAny || values.Count >= _max) break;
        }

        if (values.Count < _min || values.Count > _max)
            return RulerResult<T[]>.Failure(ErrorInfo.RawMessage(start, ErrorMessages.InvalidCount));

        return RulerResult<T[]>.Success(cursor, values.ToArray());
    }
}
