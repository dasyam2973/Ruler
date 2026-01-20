namespace Ruler;

public delegate RulerResult<T> Parser<T>(TextCursor cursor);