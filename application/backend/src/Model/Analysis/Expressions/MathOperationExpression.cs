using System;
using backend.Model.Enum;
using backend.Model.Exceptions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Model.Analysis.Expressions;

public abstract class MathOperationExpression : Expression
{
    [ForeignKey("Left")]
    public Guid? LeftId { get; set; }
    [ForeignKey("Right")]
    public Guid? RightId { get; set; }

    public KPI? Left { get; set; }

    public KPI? Right { get; set; }

    protected abstract double DoOperation(double left, double right);
    public override List<QueryReturnType> ALLOWED_QUERY_TYPES => new List<QueryReturnType> { QueryReturnType.Number };

    public override object? Evaluate(Dictionary<string, QueryResult> data) => EvaluateMathExpression(data);

    public double EvaluateMathExpression(Dictionary<string, QueryResult> queryResults)
    {
        if (Left?.Expression == null || Right?.Expression == null)
            throw new ExpressionEvaluationException("Mathoperation without left and right detected.");

        var evalLeft = Left.Expression.Evaluate(queryResults)?.ToString() ?? string.Empty;
        var evalRight = Right.Expression.Evaluate(queryResults)?.ToString() ?? string.Empty;

        if (double.TryParse(evalLeft, out var left)
            && double.TryParse(evalRight, out var right))
            return DoOperation(left, right);
        else
            throw new ExpressionEvaluationException($"Could not deduce double values from left ({evalLeft}) or right expression ({evalRight}) of MathExpression");
    }

    public override IEnumerable<string> GetRequiredQueries()
    {
        if (Left?.Expression == null || Right?.Expression == null)
            return new List<string>();

        return Left.Expression.GetRequiredQueries().Concat(Right.Expression.GetRequiredQueries());
    }

}