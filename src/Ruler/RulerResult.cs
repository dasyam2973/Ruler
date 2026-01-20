namespace Ruler;

public readonly struct RulerResult<T>
{
    private readonly T? _value;
    private readonly ErrorInfo? _errorInfo;

    public ResultType Type { get; }
    public TextCursor Cursor { get; }
    public T Value
        => !IsSuccess || _value == null ? throw new InvalidOperationException("Parsing result is not successful.") : _value;
    public ErrorInfo ErrorInfo
        => !IsFailure || _errorInfo == null ? throw new InvalidOperationException("Parsing result is not a failure.") : _errorInfo.Value;

    public bool IsSuccess => Type == ResultType.Success;
    public bool IsFailure => Type == ResultType.Failure;
    public bool EndOfSteps => Type == ResultType.Exhausted;

    public RulerResult(ResultType type, TextCursor cursor, T? value, ErrorInfo? errorInfo)
    {
        Type = type;
        Cursor = cursor;
        _value = value;
        _errorInfo = errorInfo;
    }

    public static RulerResult<T> Success(TextCursor cursor, T value)
    {
        return new(ResultType.Success, cursor, value, null);
    }

    public static RulerResult<T> Failure(ErrorInfo errorInfo)
    {
        return new(ResultType.Failure, errorInfo.Cursor, default, errorInfo);
    }

    public static RulerResult<T> Exhausted()
    {
        return new(ResultType.Exhausted, TextCursor.Empty, default, null);
    }
}
