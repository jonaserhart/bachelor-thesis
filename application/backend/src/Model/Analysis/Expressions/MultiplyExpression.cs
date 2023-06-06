namespace backend.Model.Analysis.Expressions;

public class MultiplyExpression : MathOperationExpression
{
    protected override double DoOperation(double left, double right) => left * right;
}