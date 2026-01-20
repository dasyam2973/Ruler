namespace Ruler;

[Flags]
public enum NumberParseOptions
{
    None = 0,
    AllowSign = 1 << 0,
    AllowDecimal = 1 << 1,
    AllowExponent = 1 << 2,
    AllowLeadingDot = 1 << 3,
    AllowTrailingDot = 1 << 4,

    Float = AllowSign | AllowDecimal | AllowExponent | AllowLeadingDot | AllowTrailingDot
}
