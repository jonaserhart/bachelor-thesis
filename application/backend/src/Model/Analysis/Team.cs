using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.TeamFoundation.Core.WebApi;
using Newtonsoft.Json;

namespace backend.Model.Analysis;

public class Team
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [JsonIgnore]
    public List<AnalysisModel> Models { get; set; } = new();

    public static Team From(WebApiTeam webAPiTeam)
    {
        return new Team
        {
            Id = webAPiTeam.Id,
            Name = webAPiTeam.Name,
            Description = webAPiTeam.Description
        };
    }
}