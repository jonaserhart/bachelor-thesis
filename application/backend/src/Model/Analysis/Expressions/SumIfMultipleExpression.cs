namespace backend.Model.Analysis.Expressions;

public class SumIfMultipleExpression : DoIfMultipleExpression
{
    protected override object? Process(IEnumerable<object> conditionalItems)
    {
        var numbers = new List<double>();

        foreach (var obj in conditionalItems)
        {
            if (double.TryParse(obj.ToString(), out var d))
            {
                numbers.Add(d);
            }
        }

        return numbers.Sum();
    }
}