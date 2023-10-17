using backend.Model.Enum;

namespace backend.Model.Analysis.Expressions;

public class AddExpression : MathOperationExpression
{

    protected override double DoOperation(double left, double right) => left + right;
}