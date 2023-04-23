using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace backend.Model.Users;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    [JsonIgnore]
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public IEnumerable<UserModel> UserModels { get; set; } = new List<UserModel>();
}