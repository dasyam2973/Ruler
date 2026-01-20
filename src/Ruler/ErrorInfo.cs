namespace Ruler;

public readonly struct ErrorInfo
{
    public static readonly ErrorInfo None = new(TextCursor.Empty, ErrorType.None);

    private readonly string _strArg;
    private readonly long _intArg;

    public TextCursor Cursor { get; }
    public ErrorType Type { get; }
    public string? Tag { get; }

    public string Message
    {
        get
        {
            return Type switch
            {
                ErrorType.Mismatch => ErrorMessages.Mismatch(_strArg, $"'{(char)_intArg}'"),
                ErrorType.Expected => ErrorMessages.Expected(_strArg),
                ErrorType.InsufficientInput => ErrorMessages.InsufficientInput((int)_intArg),
                ErrorType.InvalidFormat => ErrorMessages.InvalidFormat(_strArg),
                ErrorType.RawMessage => _strArg,
                _ => "Unknown parsing error.",
            };
        }
    }

    private ErrorInfo(TextCursor cursor, ErrorType type, string? tag = null, string strArg = "", long intArg = 0)
    {
        Cursor = cursor;
        Type = type;
        Tag = tag;
        _strArg = strArg;
        _intArg = intArg;
    }

    public static ErrorInfo Mismatch(TextCursor cursor, string expected, char actual)
    {
        return new(cursor, ErrorType.Mismatch, strArg: expected, intArg: actual);
    }
    public static ErrorInfo Expected(TextCursor cursor, string expected)
    {
        return new(cursor, ErrorType.Expected, strArg: expected);
    }
    public static ErrorInfo InsufficientInput(TextCursor cursor, int required)
    {
        return new(cursor, ErrorType.InsufficientInput, intArg: required);
    }
    public static ErrorInfo InvalidFormat(TextCursor cursor, string subject)
    {
        return new(cursor, ErrorType.InvalidFormat, strArg: subject);
    }
    public static ErrorInfo RawMessage(TextCursor cursor, string message)
    {
        return new(cursor, ErrorType.RawMessage, strArg: message);
    }

    public ErrorInfo WithTag(string tag)
    {
        return new(Cursor, Type, tag, _strArg, _intArg);
    }

    public override string ToString()
    {
        return $"{Message} (Line {Cursor.Line}, Column {Cursor.Column})" + (string.IsNullOrEmpty(Tag) ? "" : $" in [{Tag}]");
    }
}
