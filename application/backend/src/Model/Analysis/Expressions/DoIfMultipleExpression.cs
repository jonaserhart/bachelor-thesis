using backend.Model.Enum;
using backend.Model.Exceptions;
using System.Text.RegularExpressions;

namespace backend.Model.Analysis.Expressions;

public abstract class DoIfMultipleExpression : Expression
{
    public List<Condition> Conditions { get; set; } = new List<Condition>();
    public ConditionConnection Connection { get; set; } = ConditionConnection.All;
    public string? ExtractField { get; set; } = string.Empty;

    public override List<QueryReturnType> ALLOWED_QUERY_TYPES => new List<QueryReturnType> { QueryReturnType.NumberList, QueryReturnType.StringList, QueryReturnType.ObjectList };

    protected abstract object? Process(IEnumerable<object> conditionalItems);

    private bool ConditionIfString(object item, Condition condition)
    {
        var value = item.ToString() ?? string.Empty;
        var cv = condition.CompareValue.ToString() ?? string.Empty;
        return condition.Operator switch
        {
            CountIfOperator.IsEqual => cv == value,
            CountIfOperator.IsNotEqual => cv != value,
            CountIfOperator.Matches => Regex.IsMatch(value, cv),
            _ => false
        };
    }

    private bool ConditionIfNumber(object item, Condition condition)
    {
        var value = Convert.ToDouble(item);
        var cv = Convert.ToDouble(condition.CompareValue);

        return condition.Operator switch
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

    private bool ConditionIfObj(object item, Condition condition)
    {
        if (string.IsNullOrEmpty(condition.Field))
            throw new ExpressionEvaluationException($"Could not evaluate object list without a 'Field'.");

        var resultDict = item as Dictionary<string, object>;
        if (resultDict == null)
            throw new ExpressionEvaluationException($"Could not evaluate object list: queryresult was not an object list.");

        if (resultDict.TryGetValue(condition.Field, out var val))
        {
            if (double.TryParse(val.ToString(), out var d))
            {
                return ConditionIfNumber(d, condition);
            }
            else
            {
                return ConditionIfString(val, condition);
            }
        }
        return false;
    }

    private object Extract(object item)
    {
        if (string.IsNullOrEmpty(ExtractField))
        {
            return item;
        }
        var obj = item as Dictionary<string, object>;
        if (obj == null)
            throw new ExpressionEvaluationException($"Could not extract field from item ${item}: was not an object.");

        if (!obj.TryGetValue(ExtractField, out var fieldValue))
        {
            throw new ExpressionEvaluationException($"Could not find field {ExtractField} in object {obj} (keys: {string.Join(',', obj.Keys)})");
        }

        return fieldValue;
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
            if (queryResult.Value is not IEnumerable<Dictionary<string, object>> resultDict)
                throw new ExpressionEvaluationException($"Could not evaluate object list: queryresult was not an object list.");

            foreach (var item in resultDict)
            {
                list.Add(item);
            }
        }

        bool conditionalFunc(object item)
        {
            var result = true;

            foreach (var condition in Conditions)
            {
                bool conditionResult = false;

                conditionResult = queryResult.Type switch
                {
                    QueryReturnType.NumberList => ConditionIfNumber(item, condition),
                    QueryReturnType.StringList => ConditionIfString(item, condition),
                    QueryReturnType.ObjectList => ConditionIfObj(item, condition),
                    _ => throw new ExpressionEvaluationException("Query type not allowed")
                };

                if (Connection == ConditionConnection.All)
                {
                    result = result && conditionResult;
                }
                else // ConditionConnection.Any
                {
                    result = result || conditionResult;
                }
            }

            return result;
        }

        var filteredItems = from item in list
                            let doesCount = conditionalFunc(item)
                            where doesCount
                            select Extract(item);

        var final = Process(filteredItems);

        return final;
    }
}