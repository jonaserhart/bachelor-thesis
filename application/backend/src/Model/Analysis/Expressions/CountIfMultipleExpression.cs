namespace backend.Model.Analysis.Expressions;

public class CountIfMultipleExpression : DoIfMultipleExpression
{
    protected override object? Process(IEnumerable<object> conditionalItems) => conditionalItems.Count();
}