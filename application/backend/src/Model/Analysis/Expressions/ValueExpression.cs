using backend.Model.Analysis.WorkItems;

namespace backend.Model.Analysis.Expressions;

public class NumericValueExpression : Expression
{
    public double? Value { get; set; }
    public override object? Evaluate(List<Workitem> workItemFieldValues) => Value;
}