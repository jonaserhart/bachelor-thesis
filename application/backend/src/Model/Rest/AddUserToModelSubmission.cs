using backend.Model.Users;

namespace backend.Model.Rest;

public class AddUserToModelSubmission
{
    public string Email { get; set; } = string.Empty;
    public ModelPermission Permission { get; set; }
}
