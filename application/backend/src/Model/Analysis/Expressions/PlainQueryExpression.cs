using backend.Model.Enum;

namespace backend.Model.Analysis.Expressions;

public class PlainQueryExpression : Expression
{

    public override ExpressionResultType ReturnType => ExpressionResultType.InheritFromQuery;
    public override object? Evaluate(Dictionary<string, QueryResult> data)
    {
        var queryResult = GetQueryResultByIdOrThrow(data);

        return queryResult.Value;
    }
}