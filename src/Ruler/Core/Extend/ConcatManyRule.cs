using System.Text;

namespace Ruler.Core.Extend;

public sealed class ConcatManyRule : Rule<string>
{
    private readonly Rule<string>[] _rules;

    public ConcatManyRule(params Rule<string>[] rules)
    {
        _rules = rules;
    }

    protected override RulerResult<string> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<string>.Exhausted();

        StringBuilder sb = new();

        while (!cursor.IsEndOfInput)
        {
            for (int i = 0; i < _rules.Length; i++)
            {
                for (int j = 0; ; j++)
                {
                    RulerResult<string> elementRes = _rules[i].ApplyInternal(cursor, j);
                    if (elementRes.IsSuccess && elementRes.Cursor.Position > cursor.Position)
                    {
                        sb.Append(elementRes.Value);
                        cursor = elementRes.Cursor;
                        goto NextItem;
                    }
                    else if (elementRes.EndOfSteps) break;
                }
            }
            break;
        NextItem:;
        }

        return RulerResult<string>.Success(cursor, sb.ToString());
    }
}
