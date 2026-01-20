namespace Ruler;

public class ParseException : Exception
{
    public ParseException(ErrorInfo error) : base(error.ToString()) { }
}
