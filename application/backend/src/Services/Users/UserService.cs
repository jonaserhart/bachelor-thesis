using System.Security.Claims;
using backend.Model.Exceptions;
using backend.Model.Users;
using backend.Services.API;
using backend.Services.Database;

namespace backend.Services.Users;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
 
    public UserService(DataContext context, ILogger<UserService> logger, IApiClientFactory apiClientFactory, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _logger = logger;
        _apiClientFactory = apiClientFactory;
        _httpContextAccessor = contextAccessor;
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

    public async Task<User?> GetByIdAsync(string userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task DeleteUser(string userId)
    {
        var found = await GetByIdAsync(userId);
        if (found == null)
            throw new DbKeyNotFoundException(userId, typeof(User));
        
        _context.Users.Remove(found);

        await _context.SaveChangesAsync();
    }

    public async Task<User> GetSelfAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Could not find nameidentifier claim in token.");
        
        var user = await GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedException();
        
        return user;
    }
}