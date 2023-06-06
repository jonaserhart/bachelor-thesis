namespace backend.Model.Analysis.Expressions;

public class MinExpression : AggregateExpression
{
    protected override double AggregationFunction(IEnumerable<double> x) => x.Min();
}