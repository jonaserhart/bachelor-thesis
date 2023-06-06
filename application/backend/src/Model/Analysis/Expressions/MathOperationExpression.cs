using backend.Model.Analysis.WorkItems;
using backend.Model.Exceptions;
using backend.Model.Rest.Converters;
using Newtonsoft.Json;

namespace backend.Model.Analysis.Expressions;

public abstract class MathOperationExpression : Expression
{
    [JsonIgnore]
    public Guid? LeftId { get; set; }
    [JsonIgnore]
    public Guid? RightId { get; set; }

    [JsonConverter(typeof(ExpressionJsonConverter))]
    public Expression? Left { get; set; }

    [JsonConverter(typeof(ExpressionJsonConverter))]
    public Expression? Right { get; set; }

    protected abstract double DoOperation(double left, double right);

    public override object? Evaluate(List<Workitem> workItems)
    {
        if (Left == null || Right == null)
            throw new ExpressionEvaluationException();

        var evalLeft = Left.Evaluate(workItems)?.ToString();
        var evalRight = Right.Evaluate(workItems)?.ToString();

        if (double.TryParse(evalLeft, out var left)
            && double.TryParse(evalRight, out var right))
            return DoOperation(left, right);
        else
            throw new ExpressionEvaluationException($"Could not deduce double values from left ({evalLeft}) or right expression ({evalRight}) of MathExpression");
    }
}