using backend.Model.Enum;
using backend.Model.Exceptions;

namespace backend.Model.Analysis.Expressions;

public class CountExpression : Expression
{
    public override List<QueryReturnType> ALLOWED_QUERY_TYPES => new List<QueryReturnType> { QueryReturnType.NumberList, QueryReturnType.StringList, QueryReturnType.ObjectList };

    public override ExpressionResultType ReturnType => ExpressionResultType.Number;

    public override object? Evaluate(Dictionary<string, QueryResult> data)
    {
        var queryResult = GetQueryResultByIdOrThrow(data);

        var list = queryResult.Value as IEnumerable<object>;

        if (list == null)
            throw new ExpressionEvaluationException($"Object was not enumerable: {queryResult.Value}");

        return list.Count();
    }
}