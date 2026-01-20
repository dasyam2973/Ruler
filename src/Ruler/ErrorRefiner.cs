namespace Ruler;

public struct ErrorRefiner
{
    public bool HasValue { get; private set; }

    private ErrorInfo _current;

    public void Update(ErrorInfo errorInfo)
    {
        if (!HasValue || errorInfo.Cursor.Position > _current.Cursor.Position)
        {
            _current = errorInfo;
            HasValue = true;
        }
    }

    public readonly ErrorInfo Get()
    {
        return HasValue ? _current : ErrorInfo.None;
    }
}
