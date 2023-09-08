using backend.Model.Analysis.Graphical;
using backend.Model.Enum;

namespace backend.Model.Rest;

public class AddReportItemSubmission
{
    public GraphicalReportItemType Type { get; set; }
    public GraphicalReportItemLayout? Layout { get; set; }
}