using Ruler.Rules;

namespace RulerTest;

public class NumericTests
{
    [Theory]
    [InlineData("12ab", 0x12ab)]
    [InlineData("FFFF", 0xFFFF)]
    [InlineData("0000", 0x0000)]
    public void Hex_ShouldParseVariousValidInputs(string input, int expected)
    {
        var rule = Rules.Hex(4);
        var result = rule.Apply(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData("abc", 0)]
    [InlineData("0fbg", 3)]
    public void Hex_ShouldFail_ForInvalidInputs(string input, int failAt)
    {
        var rule = Rules.Hex(4);
        var result = rule.Apply(input);

        Assert.True(result.IsFailure);
        Assert.Equal(failAt, result.Cursor.Position);
    }
}
