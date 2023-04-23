using backend.Model.Analysis;
using Newtonsoft.Json;

namespace backend.Model.Users;

public class UserModel
{
    [JsonIgnore]
    public User? User { get; set; }
    public Guid? UserId { get; set; }
    [JsonIgnore]
    public AnalysisModel? Model { get; set; }
    public Guid? ModelId { get; set; }
    public IEnumerable<ModelPermission> Permissions { get; set; } = new List<ModelPermission>();
}