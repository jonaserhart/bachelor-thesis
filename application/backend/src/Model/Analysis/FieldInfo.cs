using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Newtonsoft.Json;
using backend.Model.Enum;

namespace backend.Model.Analysis;

public class FieldInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string ReferenceName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public WorkItemValueType Type { get; set; }
    [JsonIgnore]
    public Query? Query { get; set; }
    [JsonIgnore]
    public Guid? QueryId { get; set; }

    public override string ToString()
    {
        return $"{this.Name} [{this.ReferenceName}] ({this.Type})";
    }
}