using Ruler;
using Ruler.Rules;
using System.Text;

namespace JsonParser;

public class JsonArray : JsonValue
{
    public static readonly Rule<JsonArray> ArrayRule =
        ValueRule.SeparatedBy0(Rules.Char(',')).Between(Rules.Char('['), Rules.Char(']'))
        .Trim().Select(v => new JsonArray(v));

    private readonly JsonValue[] _values;

    public JsonArray(JsonValue[] values)
    {
        _values = values;
    }

    public override string ToString(int depth)
    {
        if (_values.Length == 0)
            return "[]";

        StringBuilder sb = new();
        sb.AppendLine("[");
        for (int i = 0; i < _values.Length; i++)
        {
            sb.Append(Indent(depth + 1)).Append(_values[i].ToString(depth + 1));
            if (i + 1 < _values.Length)
                sb.AppendLine(",");
        }
        sb.AppendLine().Append(Indent(depth)).Append(']');
        return sb.ToString();
    }
}
