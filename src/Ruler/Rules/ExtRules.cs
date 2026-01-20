using Ruler.Core;
using Ruler.Core.Extend;

namespace Ruler.Rules;

public static class ExtRules
{
    private static readonly Dictionary<char, char> _singleCharEscapeMap = new()
    {
        { 'n', '\n' }, { 'r', '\r' }, { 't', '\t' },
        { '\\', '\\' }, { '"', '"' }, { '\'', '\'' },
        { 'b', '\b' }, { 'f', '\f' }, { 'v', '\v' }, { '0', '\0' }
    };

    public static readonly Rule<char> CharEscape = new PrimitiveRule<char>(static (cursor) =>
    {
        TextCursor start = cursor;

        if (cursor.IsEndOfInput)
            return RulerResult<char>.Failure(ErrorInfo.InsufficientInput(cursor, 1));

        if (cursor.Peek() != '\\')
            return RulerResult<char>.Failure(ErrorInfo.Mismatch(cursor, "'\\'", cursor.Peek()));

        cursor = cursor.Advance();

        if (cursor.IsEndOfInput)
            return RulerResult<char>.Failure(ErrorInfo.InsufficientInput(start, 2));

        char c = cursor.Peek();
        if (!_singleCharEscapeMap.ContainsKey(c))
            return RulerResult<char>.Failure(ErrorInfo.Expected(cursor, ErrorMessages.EscapeChar));
        
        return RulerResult<char>.Success(cursor.Advance(), _singleCharEscapeMap[c]);
    });

    public static readonly Rule<char> UnicodeEscape =
        Rules.String("\\u")
        .IgnoreThen(Rules.Hex(4).Select(v => (char)v));

    public static readonly Rule<char> EscapeSequence = Rules.Choice(UnicodeEscape, CharEscape);

    public static Rule<string> ConcatMany(params Rule<string>[] rules)
        => new ConcatManyRule(rules);
}
