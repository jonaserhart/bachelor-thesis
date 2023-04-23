using System.Net.NetworkInformation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.TeamFoundation.Core.WebApi;
using Newtonsoft.Json;
using backend.Model.Rest;

namespace backend.Model.Analysis;

public class Project
{
    [Key]
    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    [JsonIgnore]
    public List<AnalysisModel> Models { get; set; } = new();

    public static Project From(TeamProjectReference tpr)
    {
        return new Project
        {
            Id = tpr.Id,
            Description = tpr.Description,
            Name = tpr.Name,
            ImageUrl = tpr.DefaultTeamImageUrl
        };
    }

    public static Project From(ProjectRequest pr)
    {
        return new Project
        {
            Id = pr.Id,
            Description = pr.Description ?? "",
            Name = pr.Name,
            ImageUrl = pr.ImageUrl ?? ""
        };
    }
}