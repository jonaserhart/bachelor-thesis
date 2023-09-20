using backend.Model.Analysis;
using Newtonsoft.Json;

namespace backend.Model.Users;

public class UserModel
{
    public User? User { get; set; }
    [JsonIgnore]
    public Guid? UserId { get; set; }
    [JsonIgnore]
    public AnalysisModel? Model { get; set; }
    [JsonIgnore]
    public Guid? ModelId { get; set; }
    public ModelPermission Permission { get; set; }
}