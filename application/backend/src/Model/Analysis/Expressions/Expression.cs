using System.ComponentModel.DataAnnotations.Schema;
using backend.Model.Enum;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using backend.Model.Exceptions;

namespace backend.Model.Analysis.Expressions;

public abstract class Expression
{
    public Guid Id { get; set; }
    public ExpressionType Type { get; set; }

    [JsonProperty("allowedQueryTypes")]
    [NotMapped]
    public virtual List<QueryReturnType> ALLOWED_QUERY_TYPES => System.Enum.GetValues<QueryReturnType>().ToList();
    public string? QueryId { get; set; }
    protected QueryResult GetQueryResultByIdOrThrow(Dictionary<string, QueryResult> data)
    {
        if (QueryId == null)
            throw new ExpressionEvaluationException("Could not evaluate CountIfExpression because QueryId was null.");

        if (!data.TryGetValue(QueryId, out var queryResult))
            throw new ExpressionEvaluationException($"Could not evaluate CountIfExpression because query with id {QueryId} was not found.");

        return queryResult;
    }
    public abstract object? Evaluate(Dictionary<string, QueryResult> data);
    public virtual IEnumerable<string> GetRequiredQueries()
    {
        if (QueryId != null)
            return new List<string> { QueryId };

        return new List<string>();
    }
}