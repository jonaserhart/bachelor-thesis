using backend.Model.Analysis.KPIs;
using backend.Model.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Model.Analysis.Expressions;

public abstract class LeftRightExpression : Expression
{
    [ForeignKey("Left")]
    public Guid? LeftId { get; set; }
    [ForeignKey("Right")]
    public Guid? RightId { get; set; }

    public KPI? Left { get; set; }

    public KPI? Right { get; set; }
    public override ExpressionResultType ReturnType => ExpressionResultType.Number;

    public override IEnumerable<string> GetRequiredQueries()
    {
        return Left?.Expression == null || Right?.Expression == null
            ? (IEnumerable<string>)new List<string>()
            : Left.Expression.GetRequiredQueries().Concat(Right.Expression.GetRequiredQueries());
    }
}
