using System.Runtime.CompilerServices;

namespace Ruler;

public readonly struct TextCursor : IEquatable<TextCursor>
{
    public static readonly TextCursor Empty = new(string.Empty);

    public string Text { get; }
    public int Position { get; }
    public int Remaining { get; }
    public int Line { get; }
    public int Column { get; }

    public bool IsEndOfInput => Remaining <= 0;

    private TextCursor(string text, int pos, int rem, int line, int column)
    {
        Text = text;
        Position = pos;
        Remaining = rem;
        Line = line;
        Column = column;
    }
    public TextCursor(string text) : this(text, 0, text.Length, 1, 1) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char Peek() => IsEndOfInput ? '\0' : Text[Position];

    public TextCursor Advance()
    {
        if (IsEndOfInput)
            throw new InvalidOperationException("Cannot advance the cursor: end of input has been reached.");

        if (Text[Position] == '\n')
        {
            return new(Text, Position + 1, Remaining - 1, Line + 1, 1);
        }
        return new(Text, Position + 1, Remaining - 1, Line, Column + 1);
    }

    public TextCursor Advance(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        int pos = Position, rem = Remaining, line = Line, column = Column;
        for (int i = 0; i < count; i++)
        {
            if (rem <= 0)
                throw new InvalidOperationException("Cannot advance the cursor: end of input has been reached.");

            if (Text[pos] == '\n')
            {
                line++; column = 0;
            }
            else
            {
                column++;
            }
            pos++; rem--;
        }
        return new(Text, pos, rem, line, column);
    }

    public TextCursor Slice(int length)
    {
        if (length < 0 || length > Remaining)
            throw new ArgumentOutOfRangeException(nameof(length));

        return new(Text, Position, length, Line, Column);
    }

    public ReadOnlySpan<char> AsSpan()
    {
        return Text.AsSpan(Position, Remaining);
    }

    public bool Equals(TextCursor other) => ReferenceEquals(Text, other.Text) && Position == other.Position;
    public override bool Equals(object? obj) => obj is TextCursor cursor && Equals(cursor);
    public override int GetHashCode() => HashCode.Combine(Text.GetHashCode(), Position);
    public static bool operator ==(TextCursor left, TextCursor right) => left.Equals(right);
    public static bool operator !=(TextCursor left, TextCursor right) => !(left == right);
}
