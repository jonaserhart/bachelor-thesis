using backend.Model.Analysis.WorkItems;
using backend.Model.Enum;

namespace backend.Model.Analysis.Expressions;

public class FieldExpression : Expression
{
    public string Field { get; set; } = string.Empty;
    public Guid QueryId { get; set; }

    public override object? Evaluate(List<Workitem> workItems)
    {
        var item = workItems.SelectMany(w => w.WorkItemFields)
                            .FirstOrDefault(d => d.Key == Field);
        if (item == null)
        {
            throw new ArgumentException($"Field '{Field}' not found in dictionary");
        }

        switch (item.Type)
        {
            case WorkItemValueType.Number:
                return double.Parse(item.Value);
            case WorkItemValueType.Boolean:
                return bool.Parse(item.Value);
            default:
                return item.Value;
        }
    }

}