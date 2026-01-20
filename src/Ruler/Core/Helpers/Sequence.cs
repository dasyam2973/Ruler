namespace Ruler.Core.Helpers;

internal static class Sequence
{
    public static RulerResult<(T1, T2)> Apply<T1, T2>(TextCursor cursor, Rule<T1> first, Rule<T2> second)
    {
        ErrorRefiner refiner = new();

        for (int i = 0; ; i++)
        {
            RulerResult<T1> firstRes = first.ApplyInternal(cursor, i);
            if (firstRes.IsSuccess)
            {
                for (int j = 0; ; j++)
                {
                    RulerResult<T2> secondRes = second.ApplyInternal(firstRes.Cursor, j);
                    if (secondRes.IsSuccess)
                    {
                        return RulerResult<(T1, T2)>.Success(secondRes.Cursor, (firstRes.Value, secondRes.Value));
                    }
                    else if (secondRes.EndOfSteps) break;
                    else if (secondRes.IsFailure) refiner.Update(secondRes.ErrorInfo);
                }
            }
            else if (firstRes.EndOfSteps) break;
            else if (firstRes.IsFailure) refiner.Update(firstRes.ErrorInfo);
        }

        return RulerResult<(T1, T2)>.Failure(refiner.Get());
    }

    public static RulerResult<(T1, T2, T3)> Apply<T1, T2, T3>(TextCursor cursor, Rule<T1> first, Rule<T2> second, Rule<T3> third)
    {
        ErrorRefiner refiner = new();

        for (int i = 0; ; i++)
        {
            RulerResult<T1> firstRes = first.ApplyInternal(cursor, i);
            if (firstRes.IsSuccess)
            {
                RulerResult<(T2, T3)> seqRes = Apply(firstRes.Cursor, second, third);
                if (seqRes.IsSuccess)
                {
                    return RulerResult<(T1, T2, T3)>.Success(seqRes.Cursor, (firstRes.Value, seqRes.Value.Item1, seqRes.Value.Item2));
                }
                else if (seqRes.IsFailure) refiner.Update(seqRes.ErrorInfo);
            }
            else if (firstRes.EndOfSteps) break;
            else if (firstRes.IsFailure) refiner.Update(firstRes.ErrorInfo);
        }

        return RulerResult<(T1, T2, T3)>.Failure(refiner.Get());
    }
}
