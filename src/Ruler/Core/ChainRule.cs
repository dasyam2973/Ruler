namespace Ruler.Core;

public sealed class ChainRule<TValue, TOperator> : Rule<TValue>
{
    private readonly Rule<TValue> _operand;
    private readonly Rule<TOperator> _op;
    private readonly Func<TValue, TOperator, TValue, TValue> _combiner;

    public ChainRule(Rule<TValue> operand, Rule<TOperator> op, Func<TValue, TOperator, TValue, TValue> combiner)
    {
        _operand = operand;
        _op = op;
        _combiner = combiner;
    }

    protected override RulerResult<TValue> Apply(TextCursor cursor, int step)
    {
        if (step != 0)
            return RulerResult<TValue>.Exhausted();

        TextCursor start = cursor;
        TValue result;
        ErrorRefiner refiner = new();

        for (int i = 0; ; i++)
        {
            RulerResult<TValue> operandRes = _operand.ApplyInternal(cursor, i);
            if (operandRes.IsSuccess)
            {
                result = operandRes.Value;
                cursor = operandRes.Cursor;
                while (true)
                {
                    for (int j = 0; ; j++)
                    {
                        RulerResult<TOperator> opRes = _op.ApplyInternal(cursor, j);
                        if (opRes.IsSuccess)
                        {
                            for (int k = 0; ; k++)
                            {
                                operandRes = _operand.ApplyInternal(opRes.Cursor, k);
                                if (operandRes.IsSuccess && operandRes.Cursor.Position > cursor.Position)
                                {
                                    result = _combiner(result, opRes.Value, operandRes.Value);
                                    cursor = operandRes.Cursor;
                                    goto NextItem;
                                }
                                else if (operandRes.EndOfSteps) break;
                            }
                        }
                        else if (opRes.EndOfSteps) break;
                    }
                    break;
                NextItem:;
                }
                return RulerResult<TValue>.Success(cursor, result);
            }
            else if (operandRes.EndOfSteps) break;
            else if (operandRes.IsFailure) refiner.Update(operandRes.ErrorInfo);
        }

        return RulerResult<TValue>.Failure(refiner.Get());
    }
}
