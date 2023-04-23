using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.TeamFoundation.Work.WebApi;

namespace backend.Model.Analysis;

public class Iteration
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public static Iteration From(TeamSettingsIteration iteration)
    {
        return new Iteration
        {
            Name = iteration.Name,
            Path = iteration.Path
        };
    }
}