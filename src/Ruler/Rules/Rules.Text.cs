using Ruler.Core;
using Ruler.Core.Helpers;
using System.Text;
using System.Text.RegularExpressions;

namespace Ruler.Rules;

public static partial class Rules
{
    public static readonly Rule<char> AnyChar = Char(c => true, "any character");

    public static readonly Rule<char> WhiteSpace = Char(char.IsWhiteSpace, "white space");

    public static readonly Rule<char> Digit = Char(CharHelper.IsDigit, "digit");

    public static readonly Rule<char> HexDigit = Char(CharHelper.IsHexDigit, "hex digit");

    public static readonly Rule<Void> WhiteSpaces = SkipWhile(char.IsWhiteSpace);

    public static Rule<char> Char(Predicate<char> predicate, string expected = "")
    {
        return new PrimitiveRule<char>(cursor =>
        {
            if (cursor.Remaining < 1)
                return RulerResult<char>.Failure(ErrorInfo.InsufficientInput(cursor, 1));

            char c = cursor.Peek();
            if (predicate(c))
                return RulerResult<char>.Success(cursor.Advance(), c);
            return RulerResult<char>.Failure(ErrorInfo.Mismatch(cursor, expected, c));
        });
    }

    public static Rule<char> Char(char c)
    {
        return Char(actual => actual == c, $"'{c}'");
    }

    public static Rule<string> String(string s, Comparison<char> charComparison, Func<char, string> getExpected)
    {
        return new PrimitiveRule<string>(cursor =>
        {
            if (cursor.Remaining < s.Length)
                return RulerResult<string>.Failure(ErrorInfo.InsufficientInput(cursor, s.Length));

            StringBuilder sb = new();
            for (int i = 0; i < s.Length; i++)
            {
                char c = cursor.Peek();
                if (charComparison(c, s[i]) != 0)
                    return RulerResult<string>.Failure(ErrorInfo.Mismatch(cursor, getExpected(s[i]), c));
                sb.Append(c);
                cursor = cursor.Advance();
            }
            return RulerResult<string>.Success(cursor, sb.ToString());
        });
    }

    public static Rule<string> String(string s)
    {
        return String(s, (x, y) => x.CompareTo(y), c => $"'{c}'");
    }

    public static Rule<string> IgnoreCase(string s)
    {
        return String(s, (x, y) => char.ToLower(x).CompareTo(char.ToLower(y)),
            c => char.IsLetter(c) ? $"'{char.ToLower(c)}' or '{char.ToUpper(c)}'" : $"'{c}'");
    }

    public static Rule<string> Regex(string pattern, string expected = "regex")
    {
        Regex regex = new(@"\G" + pattern);
        return new PrimitiveRule<string>(cursor =>
        {
            Match match = regex.Match(cursor.Text, cursor.Position);
            if (match.Success)
            {
                return RulerResult<string>.Success(cursor.Advance(match.Length), match.Value);
            }
            return RulerResult<string>.Failure(ErrorInfo.Expected(cursor, expected));
        });
    }

    public static Rule<string> TakeUntil(Predicate<char> predicate)
    {
        return new PrimitiveRule<string>(cursor =>
        {
            StringBuilder sb = new();
            while (!cursor.IsEndOfInput)
            {
                char c = cursor.Peek();
                if (predicate(c))
                    break;
                sb.Append(c);
                cursor = cursor.Advance();
            }
            return RulerResult<string>.Success(cursor, sb.ToString());
        });
    }

    public static Rule<Void> SkipUntil(Predicate<char> predicate)
    {
        return new PrimitiveRule<Void>(cursor =>
        {
            while (!cursor.IsEndOfInput)
            {
                if (predicate(cursor.Peek()))
                    break;
                cursor = cursor.Advance();
            }
            return RulerResult<Void>.Success(cursor, Void.Default);
        });
    }

    public static Rule<string> TakeWhile(Predicate<char> predicate)
    {
        return new PrimitiveRule<string>(cursor =>
        {
            StringBuilder sb = new();
            while (!cursor.IsEndOfInput)
            {
                char c = cursor.Peek();
                if (!predicate(c))
                    break;
                sb.Append(c);
                cursor = cursor.Advance();
            }
            return RulerResult<string>.Success(cursor, sb.ToString());
        });
    }

    public static Rule<Void> SkipWhile(Predicate<char> predicate)
    {
        return new PrimitiveRule<Void>(cursor =>
        {
            while (!cursor.IsEndOfInput)
            {
                if (!predicate(cursor.Peek()))
                    break;
                cursor = cursor.Advance();
            }
            return RulerResult<Void>.Success(cursor, Void.Default);
        });
    }
}
