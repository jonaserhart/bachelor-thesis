using backend.Model.Users;

namespace backend.Model.Rest;

public class UserResponse
{
    public Guid? Id { get; set; }
    public string? DisplayName { get; set; }
    public string? EMail { get; set; }

    public static UserResponse From(User user) 
    {
        return new UserResponse
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                EMail = user.EMail
            };
    }
}