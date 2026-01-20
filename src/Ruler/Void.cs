namespace Ruler;

public readonly struct Void : IEquatable<Void>
{
    public static readonly Void Default = default;

    public bool Equals(Void other) => true;
    public override bool Equals(object? obj) => obj is Void;
    public override int GetHashCode() => 0;
    public override string ToString() => "void";
    public static bool operator ==(Void left, Void right) => left.Equals(right);
    public static bool operator !=(Void left, Void right) => !(left == right);
}
