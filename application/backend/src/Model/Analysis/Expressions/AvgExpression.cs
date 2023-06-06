namespace backend.Model.Analysis.Expressions;

public class AvgExpression : AggregateExpression
{
    protected override double AggregationFunction(IEnumerable<double> x) => x.Average();
}