using backend.Model.Users;

namespace backend.Services.Users;

public interface IUserService
{
    Task<User> CreateOrUpdateUserAsync(User user);
    Task DeleteUserAsync(Guid userId);
    Task<User> GetSelfAsync();
}