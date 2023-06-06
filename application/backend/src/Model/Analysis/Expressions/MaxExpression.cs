namespace backend.Model.Analysis.Expressions;

public class MaxExpression : AggregateExpression
{
    protected override double AggregationFunction(IEnumerable<double> x) => x.Max();
}