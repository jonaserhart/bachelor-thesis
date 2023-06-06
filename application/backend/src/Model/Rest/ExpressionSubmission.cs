using backend.Model.Analysis.Expressions;
using backend.Model.Rest.Converters;
using Newtonsoft.Json;

namespace backend.Model.Rest;

public class ExpressionSubmission
{
    [JsonConverter(typeof(ExpressionJsonConverter))]
    public Expression? Expression { get; set; }
}