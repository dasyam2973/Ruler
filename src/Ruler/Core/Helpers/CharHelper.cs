namespace Ruler.Core.Helpers;

internal static class CharHelper
{
    public static bool IsDigit(char c) => '0' <= c && c <= '9';

    public static bool IsHexDigit(char c) => IsDigit(c) || ('A' <= c && c <= 'F') || ('a' <= c && c <= 'f');

    public static int ToHexInt(char c)
    {
        if ('0' <= c && c <= '9') return c - '0';
        if ('A' <= c && c <= 'F') return c - 'A' + 10;
        return c - 'a' + 10;
    }
}
