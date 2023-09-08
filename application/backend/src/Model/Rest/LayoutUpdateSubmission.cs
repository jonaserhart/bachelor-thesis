using backend.Model.Analysis.Graphical;

namespace backend.Model.Rest;

public class LayoutUpdateSubmission
{
    public GraphicalReportItemLayout? Layout { get; set; }
}