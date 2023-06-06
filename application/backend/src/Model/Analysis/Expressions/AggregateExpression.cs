using backend.Model.Analysis.WorkItems;
using backend.Model.Exceptions;
using backend.Model.Rest.Converters;
using Newtonsoft.Json;

namespace backend.Model.Analysis.Expressions;

public abstract class AggregateExpression : Expression
{
    [JsonIgnore]
    public Guid? FieldExpressionId { get; set; }

    [JsonConverter(typeof(ExpressionJsonConverter))]
    public FieldExpression? FieldExpression { get; set; }

    protected abstract double AggregationFunction(IEnumerable<double> x);

    public override object? Evaluate(List<Workitem> workItems)
    {
        if (FieldExpression == null)
            throw new ExpressionEvaluationException();
        var aggregated = AggregationFunction(workItems.Select(w => FieldExpression.Evaluate(new List<Workitem> { w }))
                                .OfType<double>());
        return aggregated;
    }
}
