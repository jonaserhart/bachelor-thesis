using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Newtonsoft.Json;

namespace backend.Model.Analysis;

public class FieldInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string ReferenceName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    [JsonIgnore]
    public Query? Query { get; set; }
    [JsonIgnore]
    public Guid? QueryId { get; set; }

    public static FieldInfo From(WorkItemField field)
    {
        return new FieldInfo
        {
            Name = field.Name,
            ReferenceName = field.ReferenceName,
            Type = field.Type.ToString(),
        };
    }
}