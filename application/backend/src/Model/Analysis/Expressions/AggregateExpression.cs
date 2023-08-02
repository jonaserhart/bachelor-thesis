using System.ComponentModel.DataAnnotations;
using backend.Model.Enum;
using backend.Model.Exceptions;

namespace backend.Model.Analysis.Expressions;

public abstract class AggregateExpression : Expression
{
    protected abstract double AggregationFunction(IEnumerable<double> x);
    public string Field { get; set; } = string.Empty;

    public override object? Evaluate(Dictionary<string, QueryResult> data)
    {
        return EvaluateAggregateExpression(data);
    }

    public override List<QueryReturnType> ALLOWED_QUERY_TYPES => new List<QueryReturnType> { QueryReturnType.NumberList, QueryReturnType.ObjectList };

    public double EvaluateAggregateExpression(Dictionary<string, QueryResult> data)
    {
        var queryResult = GetQueryResultByIdOrThrow(data);

        IEnumerable<double> values;
        if (queryResult.Type == QueryReturnType.NumberList)
        {
            var queryResultValue = queryResult.Value as IEnumerable<object>;
            if (queryResultValue == null)
                throw new ExpressionEvaluationException("Could not parse list from queryresult.");

            values = queryResultValue.Select(rV => System.Convert.ToDouble(rV));
        }
        else
        {
            var queryResultValue = queryResult.Value as IEnumerable<Dictionary<string, object>>;
            if (queryResultValue == null)
                throw new ExpressionEvaluationException("Could not parse dictionary from queryresult.");

            values = queryResultValue.Select(w =>
            {
                var val = w.GetValueOrDefault(Field);
                if (val == null) return double.NaN;
                return System.Convert.ToDouble(val);
            })
            .Where(x => !double.IsNaN(x));
        }

        var aggregated = AggregationFunction(values);
        return aggregated;
    }
}
