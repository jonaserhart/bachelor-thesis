using backend.Model.Analysis.WorkItems;
using backend.Model.Enum;

namespace backend.Model.Analysis.Expressions;

public class CountIfExpression : Expression
{
    public string Field { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public override object? Evaluate(List<Workitem> workItems)
    {
        var count = 0;

        foreach (var workItem in workItems)
        {
            var item = workItem.WorkItemFields.FirstOrDefault(d => d.Key == Field);
            if (item != null)
            {
                switch (item.Type)
                {
                    case WorkItemValueType.Number:
                        if (CompareNumericValues(double.Parse(item.Value), double.Parse(Value), Operator))
                        {
                            count++;
                        }
                        break;
                    case WorkItemValueType.Boolean:
                        if (CompareBooleanValues(bool.Parse(item.Value), bool.Parse(Value), Operator))
                        {
                            count++;
                        }
                        break;
                    default:
                        if (CompareStringValues(item.Value, Value, Operator))
                        {
                            count++;
                        }
                        break;
                }
            }
        }

        return count;
    }

    private static bool CompareNumericValues(double value1, double value2, string op)
    {
        return op switch
        {
            "==" => value1 == value2,
            "!=" => value1 != value2,
            "<" => value1 < value2,
            "<=" => value1 <= value2,
            ">" => value1 > value2,
            ">=" => value1 >= value2,
            _ => throw new ArgumentException($"Invalid operator: {op}"),
        };
    }

    private static bool CompareBooleanValues(bool value1, bool value2, string op)
    {
        return op switch
        {
            "==" => value1 == value2,
            "!=" => value1 != value2,
            _ => throw new ArgumentException($"Invalid operator: {op}"),
        };
    }

    private static bool CompareStringValues(string value1, string value2, string op)
    {
        return op switch
        {
            "==" => value1 == value2,
            "!=" => value1 != value2,
            _ => throw new ArgumentException($"Invalid operator: {op}"),
        };
    }
}
