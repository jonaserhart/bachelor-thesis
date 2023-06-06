namespace backend.Model.Analysis.Expressions;

public class DivExpression : MathOperationExpression
{
    protected override double DoOperation(double left, double right)
    {
        if (right == 0)
        {
            throw new DivideByZeroException();
        }
        else return left / right;
    }
}