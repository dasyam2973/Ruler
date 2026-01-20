using Ruler.Rules;

namespace RulerTest;

public class EscapeTests
{
    [Theory]
    [InlineData(@"\n", '\n')]
    [InlineData(@"\t", '\t')]
    [InlineData(@"\r", '\r')]
    [InlineData(@"\\", '\\')]
    [InlineData(@"\""", '\"')]
    public void CharEscape_ShouldParseStandardEscapes(string input, char expected)
    {
        var rule = ExtRules.CharEscape;
        var result = rule.Apply(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
        Assert.True(result.Cursor.IsEndOfInput);
    }

    [Theory]
    [InlineData(@"\u0041", 'A')]
    [InlineData(@"\u0020", ' ')]
    [InlineData(@"\uac00", '가')]
    [InlineData(@"\uD55C", '한')]
    public void UnicodeEscape_ShouldParseCorrectCharacter(string input, char expected)
    {
        var rule = ExtRules.UnicodeEscape;
        var result = rule.Apply(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
        Assert.True(result.Cursor.IsEndOfInput);
    }

    [Theory]
    [InlineData(@"\z", 1)]
    [InlineData(@"\u123", 2)]
    [InlineData(@"\uGGGG", 2)]
    public void Escape_ShouldFail_OnInvalidInput(string input, int failAt)
    {
        var rule = ExtRules.EscapeSequence;
        var result = rule.Apply(input);

        Assert.True(result.IsFailure);
        Assert.Equal(failAt, result.Cursor.Position);
    }
}
