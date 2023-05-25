using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Model.Analysis.WorkItems;

public class Workitem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public ICollection<WorkItemKeyValue> WorkItemFields { get; set; } = new List<WorkItemKeyValue>();
}