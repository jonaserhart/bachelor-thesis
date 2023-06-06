using System.Security.Claims;
using backend.Model.Exceptions;
using backend.Model.Users;
using backend.Services.API;
using backend.Services.Database;
using Microsoft.EntityFrameworkCore;

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
    public async Task<User> CreateOrUpdateUserAsync(User user)
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

    /// <summary>
    /// Method to get a User by Id
    /// </summary>
    /// <param name="userId">UserId to search for</param>
    /// <returns>A User if the Id was found, null otherwise</returns>
    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _context.Users
            .Include(x => x.UserModels).FirstOrDefaultAsync(x => x.Id == userId);
    }

    /// <summary>
    /// Method to delete a User from the database
    /// </summary>
    /// <param name="userId">Id of the user to delete</param>
    /// <exception cref="DbKeyNotFoundException" />
    /// <returns></returns>
    public async Task DeleteUserAsync(Guid userId)
    {
        var found = await GetByIdAsync(userId);
        if (found == null)
            throw new DbKeyNotFoundException(userId, typeof(User));

        _context.Users.Remove(found);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the currently authenticated user via HttpContext
    /// </summary>
    /// <exception cref="UnauthorizedException" />
    /// <returns>The currently authenticated user</returns>
    public async Task<User> GetSelfAsync()
    {
        var userId = _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)
            || !Guid.TryParse(userId, out var id))
            throw new UnauthorizedException("Could not find nameidentifier claim in token.");

        var user = await GetByIdAsync(id);
        if (user == null)
            throw new UnauthorizedException($"Could not find User with Id {id} in database.");

        return user;
    }
}