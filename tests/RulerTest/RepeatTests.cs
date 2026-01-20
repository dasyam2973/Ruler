using Ruler;
using Ruler.Rules;

namespace RulerTest;

public class RepeatTests
{
    [Fact]
    public void Many_ShouldParseMultipleItems()
    {
        var rule = Rules.Char('a').Many0();
        var result = rule.Apply("aaa");

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Length);
        Assert.All(result.Value, c => Assert.Equal('a', c));
    }

    [Fact]
    public void Many_ShouldSucceed_OnEmptyInput()
    {
        var rule = Rules.Char('a').Many0();
        var result = rule.Apply("");

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public void AtLeastOnce_ShouldFail_OnEmptyInput()
    {
        var rule = Rules.Char('a').Many1();
        var result = rule.Apply("");

        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("1,2,3", 3)]
    [InlineData("1", 1)]
    [InlineData("", 0)]
    [InlineData("0, 1", 1)]
    public void SeparatedBy_ShouldParseWithDelimiter(string input, int expectedCount)
    {
        var rule = Rules.Digit.SeparatedBy0(Rules.Char(','));
        var result = rule.Apply(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCount, result.Value.Length);
    }

    [Theory]
    [InlineData("abc", true)]
    [InlineData("ab", false)]
    [InlineData("abcd", true)]
    public void Repeat_ShouldParseExactCount(string input, bool shouldSuccess)
    {
        var rule = Rules.AnyChar.Repeat(3);
        var result = rule.Apply(input);

        Assert.Equal(shouldSuccess, result.IsSuccess);
        if (shouldSuccess)
        {
            Assert.Equal(3, result.Value.Length);
        }
    }
}
