using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Model.Users;

public class RefreshToken
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Token { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public User? User { get; set; }
    public string? UserId { get; set; }
}