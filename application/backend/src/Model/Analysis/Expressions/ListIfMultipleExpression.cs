
using backend.Model.Enum;

namespace backend.Model.Analysis.Expressions;

public class ListIfMultipleExpression : DoIfMultipleExpression
{
    public override ExpressionResultType ReturnType => ExpressionResultType.InheritFromQuery;
    protected override object? Process(IEnumerable<object> conditionalItems) => conditionalItems;
}