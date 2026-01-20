using Ruler;
using Ruler.Rules;

namespace JsonParser;

public abstract class JsonValue
{
    protected static readonly Rule<JsonValue<Unit>> NullRule =
        Rules.Token("null", new JsonValue<Unit>(new()));

    protected static readonly Rule<JsonValue<JsonBool>> BoolRule =
        Rules.Token("true", new JsonValue<JsonBool>(new(true)))
        .Or(Rules.Token("false", new JsonValue<JsonBool>(new(false))));

    protected static readonly Rule<JsonValue<double>> NumberRule =
        Rules.Float64(NumberParseOptions.AllowSign | NumberParseOptions.AllowDecimal | NumberParseOptions.AllowExponent)
        .Trim().Select(v => new JsonValue<double>(v));

    protected static readonly Rule<JsonValue<JsonString>> StringRule =
        ExtRules.ConcatMany(
            ExtRules.EscapeSequence.Select(c => c.ToString()),
            Rules.Char(c => c != '"').Select(c => c.ToString()))
        .Between(Rules.Char('"'), Rules.Char('"'))
        .Trim().Select(v => new JsonValue<JsonString>(new(v)));

    public static readonly Rule<JsonValue> ValueRule =
        Rules.Lazy(() => Rules.Choice(
            NullRule.As<JsonValue>(),
            BoolRule.As<JsonValue>(),
            NumberRule.As<JsonValue>(),
            StringRule.As<JsonValue>(),
            JsonArray.ArrayRule.As<JsonValue>(),
            JsonObject.ObjectRule.As<JsonValue>())
        .Trim());

    public abstract string ToString(int depth);

    public override string ToString()
    {
        return ToString(0);
    }

    protected static string Indent(int depth)
    {
        return new(' ', 2 * depth);
    }
}
public class JsonValue<T> : JsonValue
{
    public T Value { get; }

    public JsonValue(T value)
    {
        Value = value;
    }

    public override string ToString(int depth)
    {
        return Value?.ToString() ?? string.Empty;
    }
}
