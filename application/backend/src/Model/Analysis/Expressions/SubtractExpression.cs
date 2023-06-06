namespace backend.Model.Analysis.Expressions;

public class SubtractExpression : MathOperationExpression
{
    protected override double DoOperation(double left, double right) => left - right;
}