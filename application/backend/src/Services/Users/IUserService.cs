using backend.Model.Users;

namespace backend.Services.Users;

public interface IUserService
{
    Task<User> CreateOrUpdateUser(User user);
    Task DeleteUser(string userId);
}