using System.Text.RegularExpressions;

using backend.Model.Enum;
using backend.Model.Exceptions;

namespace backend.Model.Analysis.Expressions;

public class CountIfExpression : Expression
{
    public CountIfOperator Operator { get; set; }
    public string CompareValue { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public override List<QueryReturnType> ALLOWED_QUERY_TYPES => new List<QueryReturnType> { QueryReturnType.NumberList, QueryReturnType.StringList, QueryReturnType.ObjectList };

    private bool CountIfString(object item)
    {
        var value = item.ToString() ?? string.Empty;
        var cv = CompareValue.ToString() ?? string.Empty;
        return Operator switch
        {
            CountIfOperator.IsEqual => cv == value,
            CountIfOperator.IsNotEqual => cv != value,
            CountIfOperator.Matches => Regex.IsMatch(value, cv),
            _ => false
        };
    }

    private bool CountIfNumber(object item)
    {
        var value = System.Convert.ToDouble(item);
        var cv = System.Convert.ToDouble(CompareValue);

        return Operator switch
        {
            CountIfOperator.IsEqual => cv == value,
            CountIfOperator.IsNotEqual => cv != value,
            CountIfOperator.IsLess => cv > value,
            CountIfOperator.IsLessOrEqual => cv >= value,
            CountIfOperator.IsMore => cv < value,
            CountIfOperator.IsMoreOrEqual => cv <= value,
            _ => false
        };
    }

    public override object? Evaluate(Dictionary<string, QueryResult> data)
    {
        var queryResult = GetQueryResultByIdOrThrow(data);

        List<object>? list = new List<object>();

        if (queryResult.Type == QueryReturnType.NumberList || queryResult.Type == QueryReturnType.StringList)
        {
            list = (queryResult.Value as IEnumerable<object>)?.ToList();
        }
        else
        {
            if (string.IsNullOrEmpty(Field))
                throw new ExpressionEvaluationException($"Could not evaluate object list without a 'Field'.");

            var resultDict = queryResult.Value as IEnumerable<Dictionary<string, object>>;
            if (resultDict == null)
                throw new ExpressionEvaluationException($"Could not evaluate object list: queryresult was not an object list.");

            foreach (var item in resultDict)
            {
                if (item.TryGetValue(Field, out var val))
                {
                    list.Add(val);
                }
            }
        }

        if (list == null)
            throw new ExpressionEvaluationException($"Object was not enumerable: {queryResult.Value}");

        Func<object, bool> countIfFunc = queryResult.Type switch
        {
            QueryReturnType.NumberList => CountIfNumber,
            QueryReturnType.StringList => CountIfString,
            QueryReturnType.ObjectList => CountIfString,
            _ => throw new ExpressionEvaluationException("Query type not allowed")
        };

        var count = (from item in list
                     let doesCount = countIfFunc(item)
                     where doesCount
                     select item).Count();

        return count;
    }
}