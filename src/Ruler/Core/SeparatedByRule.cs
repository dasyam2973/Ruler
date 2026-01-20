namespace Ruler.Core;

public sealed class SeparatedByRule<T, U> : Rule<T[]>
{
    private readonly Rule<T> _element;
    private readonly Rule<U> _separator;
    private readonly int _min;
    private readonly int _max;

    public SeparatedByRule(Rule<T> element, Rule<U> separator, int min = 0, int max = int.MaxValue)
    {
        _element = element;
        _separator = separator;
        _min = min;
        _max = max;
    }

    protected override RulerResult<T[]> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<T[]>.Exhausted();

        TextCursor start = cursor;
        List<T> values = new();

        bool hasFirstElement = false;
        for (int i = 0; ; i++)
        {
            RulerResult<T> elementRes = _element.ApplyInternal(cursor, i);
            if (elementRes.IsSuccess && elementRes.Cursor.Position > cursor.Position)
            {
                hasFirstElement = true;
                values.Add(elementRes.Value);
                cursor = elementRes.Cursor;
                break;
            }
            else if (elementRes.EndOfSteps) break;
        }

        bool continueMatch = hasFirstElement;
        while (continueMatch && values.Count < _max)
        {
            continueMatch = false;
            for (int i = 0; ; i++)
            {
                RulerResult<U> separatorRes = _separator.ApplyInternal(cursor, i);
                if (separatorRes.IsSuccess)
                {
                    for (int j = 0; ; j++)
                    {
                        RulerResult<T> elementRes = _element.ApplyInternal(separatorRes.Cursor, j);
                        if (elementRes.IsSuccess && elementRes.Cursor.Position > cursor.Position)
                        {
                            continueMatch = true;
                            values.Add(elementRes.Value);
                            cursor = elementRes.Cursor;
                            goto NextItem;
                        }
                        else if (elementRes.EndOfSteps) break;
                    }
                }
                else if (separatorRes.EndOfSteps) break;
            }
        NextItem:;
        }

        if (values.Count < _min || values.Count > _max)
            return RulerResult<T[]>.Failure(ErrorInfo.RawMessage(start, ErrorMessages.InvalidCount));

        return RulerResult<T[]>.Success(cursor, values.ToArray());
    }
}
