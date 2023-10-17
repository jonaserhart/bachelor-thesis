using backend.Model.Analysis;

namespace backend.Model.Rest;

public class GraphicalPropertiesSubmission
{
    public List<string> ListFields { get; set; } = new List<string>();
    public List<LabelAndValue> ListFieldsWithLabels { get; set; } = new List<LabelAndValue>();
}