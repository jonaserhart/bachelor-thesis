using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Model.Users;

namespace backend.Model.Analysis;

public class AnalysisModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Project? Project { get; set; }
    public Guid? ProjectId { get; set; }
    public Team? Team { get; set; }
    public Guid? TeamId { get; set; }
    public IEnumerable<Query> Queries { get; set; } = new List<Query>();
    public IEnumerable<UserModel> ModelUsers { get; set; } = new List<UserModel>();

}