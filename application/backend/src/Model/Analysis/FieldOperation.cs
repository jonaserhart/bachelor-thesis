using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace backend.Model.Analysis;

[Owned]
public class FieldOperation
{
    public string Name { get; set; } = string.Empty;
    public string ReferenceName { get; set; } = string.Empty;

    public static FieldOperation? From(WorkItemFieldOperation operation)
    {
        if (operation == null)
            return null;

        return new FieldOperation
        {
            Name = operation.Name,
            ReferenceName = operation.ReferenceName
        };
    }
}