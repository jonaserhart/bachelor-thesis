namespace backend.Model.Analysis.Expressions;

public class NumericValueExpression : Expression
{
    public double? Value { get; set; }
    public override object? Evaluate(Dictionary<string, QueryResult> data) => EvaluateNumericValueExpression();
    public double EvaluateNumericValueExpression() => Value ?? double.NaN;
    public override IEnumerable<string> GetRequiredQueries() => new List<string>();
}