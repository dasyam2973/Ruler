namespace JsonParser;

class Program
{
    static void Main()
    {
        string json = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "../../../sample.json"));
        JsonValue jsonValue = JsonValue.ValueRule.ApplyOrThrow(json);
        Console.WriteLine(jsonValue);
    }
}
