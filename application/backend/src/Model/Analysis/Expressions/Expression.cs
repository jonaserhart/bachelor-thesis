using backend.Model.Analysis.WorkItems;
using backend.Model.Enum;

namespace backend.Model.Analysis.Expressions;

public abstract class Expression
{
    public Guid Id { get; set; }
    public ExpressionType Type { get; set; }
    public abstract object? Evaluate(List<Workitem> workItems);
}
