namespace backend.Model.Analysis.Expressions;

public class SumExpression : AggregateExpression
{
    protected override double AggregationFunction(IEnumerable<double> x) => x.Sum();
}