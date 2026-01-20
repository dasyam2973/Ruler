namespace Ruler;

internal static class ErrorMessages
{
    public static string Digit => "digit";
    public static string DigitAfterPoint => "digit after decimal point";
    public static string DigitAfterExponent => "digit after exponent";
    public static string DigitNoSign => "digit (no sign allowed)";
    public static string Float => "floating-point number";
    public static string EndOfInput => "end of input";
    public static string HexDigit => "hex digit";
    public static string EscapeChar => "valid escape sequence character";

    public static string OutOfRange => "Value is out of the allowed range.";
    public static string InvalidCount => "The number of elements is out of range.";
    public static string PredicateFailure => "Value does not satisfy the condition.";

    public static string Mismatch(string expected, string actual)
        => $"Expected {expected}, but found {actual}.";

    public static string Expected(string expected)
        => $"Expected {expected}.";

    public static string InsufficientInput(int required)
        => $"Unexpected end of input. Expected at least {required} characters.";

    public static string InvalidFormat(string subject)
        => $"Invalid {subject} format.";
}
