# 📏 Ruler
A lightweight and simple parser combinator library for C#.

### Example
```csharp
Rule<int[]> intsRule = Rules.Int32.SeparatedBy0(Rules.Char(','));
var result = intsRule.Apply("1,2,3,4,5,100,-15");

Console.WriteLine(string.Join(' ', result.Value));
// -> 1 2 3 4 5 100 -15
```

### Status
⚠️ Note: This project is currently in early development. APIs are subject to change.