using backend.Model.Enum;

namespace backend.Model.Analysis.WorkItems;

public class WorkItemKeyValue
{
    public string Key { get; set; } = string.Empty;
    public WorkItemValueType Type { get; set; }
    public string Value { get; set; } = string.Empty;
}