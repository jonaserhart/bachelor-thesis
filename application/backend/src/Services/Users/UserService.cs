using backend.Model.Exceptions;
using backend.Model.Users;
using backend.Services.Database;

namespace backend.Services.Users;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(DataContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Method to create and update a user (user is created if id is not found)
    /// </summary>
    /// <param name="user">User to update or create</param>
    /// <returns>Created or updated user</returns>
    public async Task<User> CreateOrUpdateUser(User user)
    {
        var existing = await _context.Users.FindAsync(user.Id);

        if (existing != null)
        {
            _logger.LogDebug($"Updating user {user.Id}.");
            existing.DisplayName = user.DisplayName;
            existing.EMail = user.EMail;
        }
        else 
        {
            _logger.LogDebug($"Creating user {user.Id}.");
            existing = (await _context.Users.AddAsync(user)).Entity;
        }

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteUser(string userId)
    {
        var found = await _context.Users.FindAsync(userId);
        if (found == null)
            throw new DbKeyNotFoundException(userId, typeof(User));
        
        _context.Users.Remove(found);

        await _context.SaveChangesAsync();
    }
}