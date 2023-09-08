using Newtonsoft.Json;

namespace backend.Model.Analysis.Expressions;

public class Condition
{
    public Guid Id { get; set; }
    public Guid ExpressionId { get; set; }
    [JsonIgnore]
    public DoIfMultipleExpression? Expression { get; set; }
    public CountIfOperator Operator { get; set; }
    public string CompareValue { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
}