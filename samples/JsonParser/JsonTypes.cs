namespace JsonParser;

public readonly struct Unit
{
    public override string ToString()
    {
        return "null";
    }
}
public readonly struct JsonBool
{
    public readonly bool Value;

    public JsonBool(bool value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value ? "true" : "false";
    }
}
public readonly struct JsonString
{
    public readonly string Value;

    public JsonString(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return '"' + Value + '"';
    }
}
