using Ruler;
using Ruler.Rules;
using System.Text;

namespace JsonParser;

public class JsonObject : JsonValue
{
    private static readonly Rule<KeyValuePair<JsonString, JsonValue>> PairRule =
        Rules.Sequence(StringRule, Rules.Char(':'), ValueRule)
        .Trim().Select(v => new KeyValuePair<JsonString, JsonValue>(v.Item1.Value, v.Item3));

    public static readonly Rule<JsonObject> ObjectRule =
        PairRule.SeparatedBy0(Rules.Char(',')).Between(Rules.Char('{'), Rules.Char('}'))
        .Trim().Select(v => new JsonObject(v.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)));

    private readonly Dictionary<JsonString, JsonValue> _keyValuePairs;

    public JsonObject(Dictionary<JsonString, JsonValue> keyValuePairs)
    {
        _keyValuePairs = keyValuePairs;
    }

    public override string ToString(int depth)
    {
        if (_keyValuePairs.Count == 0)
            return "{}";

        StringBuilder sb = new();
        sb.AppendLine("{");
        int count = 0;
        foreach (var kvp in _keyValuePairs)
        {
            sb.Append(Indent(depth + 1)).Append($"{kvp.Key}: {kvp.Value.ToString(depth + 1)}");
            if (++count < _keyValuePairs.Count)
                sb.AppendLine(",");
        }
        sb.AppendLine().Append(Indent(depth)).Append('}');
        return sb.ToString();
    }
}
