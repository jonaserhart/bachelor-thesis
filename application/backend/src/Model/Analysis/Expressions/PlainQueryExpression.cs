namespace backend.Model.Analysis.Expressions;

public class PlainQueryExpression : Expression
{

    public override object? Evaluate(Dictionary<string, QueryResult> data)
    {
        var queryResult = GetQueryResultByIdOrThrow(data);

        return queryResult.Value;
    }
}