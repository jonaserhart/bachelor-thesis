using System;
using backend.Model.Enum;
using backend.Model.Exceptions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Model.Analysis.KPIs;

namespace backend.Model.Analysis.Expressions;

public abstract class MathOperationExpression : LeftRightExpression
{
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
}