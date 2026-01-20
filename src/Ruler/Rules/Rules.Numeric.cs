using Ruler.Core;
using Ruler.Core.Helpers;
using System.Globalization;
using System.Numerics;

namespace Ruler.Rules;

public static partial class Rules
{
    public static readonly Parser<TextCursor> RawInteger = static (cursor) =>
    {
        TextCursor start = cursor;

        if (cursor.Peek() == '-' || cursor.Peek() == '+')
            cursor = cursor.Advance();

        if (cursor.IsEndOfInput)
            return RulerResult<TextCursor>.Failure(ErrorInfo.InsufficientInput(cursor, 1));

        if (!char.IsDigit(cursor.Peek()))
            return RulerResult<TextCursor>.Failure(ErrorInfo.Mismatch(cursor, "digit", cursor.Peek()));

        cursor = cursor.Advance();
        while (!cursor.IsEndOfInput)
        {
            char c = cursor.Peek();
            if (!CharHelper.IsDigit(c)) break;
            cursor = cursor.Advance();
        }
        return RulerResult<TextCursor>.Success(cursor, start.Slice(cursor.Position - start.Position));
    };

    public static RulerResult<TextCursor> RawDecimal(TextCursor cursor, NumberParseOptions options = NumberParseOptions.Float)
    {
        TextCursor start = cursor;

        if (cursor.Peek() == '-' || cursor.Peek() == '+')
        {
            if (options.HasFlag(NumberParseOptions.AllowSign))
                cursor = cursor.Advance();
            else
                return RulerResult<TextCursor>.Failure(ErrorInfo.Expected(cursor, ErrorMessages.DigitNoSign));
        }

        bool hasAnyDigit = false;
        while (!cursor.IsEndOfInput)
        {
            char c = cursor.Peek();
            if (!CharHelper.IsDigit(c)) break;
            hasAnyDigit = true;
            cursor = cursor.Advance();
        }

        if (cursor.Peek() == '.')
        {
            if (options.HasFlag(NumberParseOptions.AllowDecimal) && (hasAnyDigit || options.HasFlag(NumberParseOptions.AllowLeadingDot)))
            {
                cursor = cursor.Advance();

                bool hasDecimalDigit = false;
                while (!cursor.IsEndOfInput)
                {
                    char c = cursor.Peek();
                    if (!CharHelper.IsDigit(c)) break;
                    hasAnyDigit = true;
                    hasDecimalDigit = true;
                    cursor = cursor.Advance();
                }

                if (!hasDecimalDigit && !options.HasFlag(NumberParseOptions.AllowTrailingDot))
                    return RulerResult<TextCursor>.Failure(ErrorInfo.Expected(cursor, ErrorMessages.DigitAfterPoint));
            }
        }

        if (!hasAnyDigit)
            return RulerResult<TextCursor>.Failure(ErrorInfo.Expected(cursor, ErrorMessages.Digit));

        if (options.HasFlag(NumberParseOptions.AllowExponent) && (cursor.Peek() == 'e' || cursor.Peek() == 'E'))
        {
            cursor = cursor.Advance();
            if (cursor.Peek() == '-' || cursor.Peek() == '+')
            {
                if (options.HasFlag(NumberParseOptions.AllowSign))
                    cursor = cursor.Advance();
                else
                    return RulerResult<TextCursor>.Failure(ErrorInfo.Expected(cursor, ErrorMessages.DigitNoSign));
            }

            bool hasExpDigit = false;
            while (!cursor.IsEndOfInput)
            {
                char c = cursor.Peek();
                if (!CharHelper.IsDigit(c)) break;
                hasExpDigit = true;
                cursor = cursor.Advance();
            }

            if (!hasExpDigit)
                return RulerResult<TextCursor>.Failure(ErrorInfo.Expected(cursor, ErrorMessages.DigitAfterExponent));
        }

        return RulerResult<TextCursor>.Success(cursor, start.Slice(cursor.Position - start.Position));
    }

    public static readonly Rule<int> Int32 = RangedInt(int.MinValue, int.MaxValue);

    public static readonly Rule<long> Int64 = RangedLong(long.MinValue, long.MaxValue);

    public static readonly Rule<BigInteger> BigInteger = new PrimitiveRule<BigInteger>(static (cursor) =>
    {
        TextCursor start = cursor;
        RulerResult<TextCursor> parseResult = RawInteger(cursor);

        if (parseResult.IsFailure)
            return RulerResult<BigInteger>.Failure(parseResult.ErrorInfo);

        BigInteger value = System.Numerics.BigInteger.Parse(parseResult.Value.AsSpan(), NumberStyles.Integer, CultureInfo.InvariantCulture);
        return RulerResult<BigInteger>.Success(parseResult.Cursor, value);
    });

    public static Rule<int> RangedInt(int min, int max)
    {
        if (min > max)
            throw new ArgumentOutOfRangeException(nameof(min), "'min' cannot be greater than 'max'");

        return new PrimitiveRule<int>(cursor =>
        {
            TextCursor start = cursor;
            RulerResult<TextCursor> parseResult = RawInteger(cursor);

            if (parseResult.IsFailure)
                return RulerResult<int>.Failure(parseResult.ErrorInfo);

            if (int.TryParse(parseResult.Value.AsSpan(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                if (value < min || value > max)
                    return RulerResult<int>.Failure(ErrorInfo.RawMessage(start, ErrorMessages.OutOfRange));
                return RulerResult<int>.Success(parseResult.Cursor, value);
            }
            return RulerResult<int>.Failure(ErrorInfo.RawMessage(start, ErrorMessages.OutOfRange));
        });
    }

    public static Rule<long> RangedLong(long min, long max)
    {
        if (min > max)
            throw new ArgumentOutOfRangeException(nameof(min), "'min' cannot be greater than 'max'");

        return new PrimitiveRule<long>(cursor =>
        {
            TextCursor start = cursor;
            RulerResult<TextCursor> parseResult = RawInteger(cursor);

            if (parseResult.IsFailure)
                return RulerResult<long>.Failure(parseResult.ErrorInfo);

            if (long.TryParse(parseResult.Value.AsSpan(), NumberStyles.Integer, CultureInfo.InvariantCulture, out long value))
            {
                if (value < min || value > max)
                    return RulerResult<long>.Failure(ErrorInfo.RawMessage(start, ErrorMessages.OutOfRange));
                return RulerResult<long>.Success(parseResult.Cursor, value);
            }
            return RulerResult<long>.Failure(ErrorInfo.RawMessage(start, ErrorMessages.OutOfRange));
        });
    }

    public static Rule<float> Float32(NumberParseOptions options = NumberParseOptions.Float)
    {
        return new PrimitiveRule<float>(cursor =>
        {
            TextCursor start = cursor;
            RulerResult<TextCursor> parseResult = RawDecimal(cursor, options);

            if (parseResult.IsFailure)
                return RulerResult<float>.Failure(parseResult.ErrorInfo);

            if (float.TryParse(parseResult.Value.AsSpan(), NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                return RulerResult<float>.Success(parseResult.Cursor, value);

            return RulerResult<float>.Failure(ErrorInfo.InvalidFormat(start, ErrorMessages.Float));
        });
    }

    public static Rule<double> Float64(NumberParseOptions options = NumberParseOptions.Float)
    {
        return new PrimitiveRule<double>(cursor =>
        {
            TextCursor start = cursor;
            RulerResult<TextCursor> parseResult = RawDecimal(cursor, options);

            if (parseResult.IsFailure)
                return RulerResult<double>.Failure(parseResult.ErrorInfo);

            if (double.TryParse(parseResult.Value.AsSpan(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                return RulerResult<double>.Success(parseResult.Cursor, value);

            return RulerResult<double>.Failure(ErrorInfo.InvalidFormat(start, ErrorMessages.Float));
        });
    }

    public static Rule<int> Hex(int count)
    {
        return new PrimitiveRule<int>(cursor =>
        {
            TextCursor start = cursor;
            int result = 0;

            for (int i = 0; i < count; i++)
            {
                if (cursor.IsEndOfInput)
                    return RulerResult<int>.Failure(ErrorInfo.InsufficientInput(start, count));

                char c = cursor.Peek();
                if (!CharHelper.IsHexDigit(c))
                    return RulerResult<int>.Failure(ErrorInfo.Expected(cursor, ErrorMessages.HexDigit));

                result = result << 4 | CharHelper.ToHexInt(c);
                cursor = cursor.Advance();
            }

            return RulerResult<int>.Success(cursor, result);
        });
    }
}
