using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Model.Users;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public List<RefreshToken> RefreshTokens = new List<RefreshToken>();
}