
using backend.Model.Analysis.Expressions;
using backend.Model.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace backend.Model.Rest.Converters;

public class ExpressionJsonConverter : JsonConverter<Expression>
{
    public override bool CanWrite => false;
    public override void WriteJson(JsonWriter writer, Expression? value, JsonSerializer serializer) => throw new NotImplementedException();

    public override Expression? ReadJson(JsonReader reader, Type objectType, Expression? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);

        if (jsonObject.TryGetValue("type", out var typeToken) && typeToken.Type == JTokenType.String)
        {
            var typeName = typeToken.Value<string>();

            if (System.Enum.TryParse<ExpressionType>(typeName, out var expressionType))
            {
                Expression? expression = expressionType switch
                {
                    ExpressionType.Add => new AddExpression(),
                    ExpressionType.Avg => new AvgExpression(),
                    ExpressionType.CountIf => new CountIfExpression(),
                    ExpressionType.Div => new DivExpression(),
                    ExpressionType.Field => new FieldExpression(),
                    ExpressionType.Max => new MaxExpression(),
                    ExpressionType.Min => new MinExpression(),
                    ExpressionType.Multiply => new MultiplyExpression(),
                    ExpressionType.Subtract => new SubtractExpression(),
                    ExpressionType.Sum => new SumExpression(),
                    ExpressionType.Value => new NumericValueExpression(),
                    _ => throw new JsonException($"Unknown type {expressionType}")
                };

                serializer.Populate(jsonObject.CreateReader(), expression);

                return expression;
            }
            else
            {
                throw new JsonSerializationException($"Invalid expression type: {typeName}");
            }
        }

        throw new JsonSerializationException("Invalid expression JSON");
    }
}