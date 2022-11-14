using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Services;

[Injectable(typeof(IUserService))]
public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly Features _features;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(ILogger<UserService> logger, UserManager<User> userManager, IOptions<Features> features, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _userManager = userManager;
        _features = features.Value;
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<User?> LookupUserByName(string username) => await _userManager.FindByNameAsync(username);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="credentials"></param>
    /// <returns></returns>
    public async Task<ClaimsIdentity> AuthenticateUser(Credential credentials)
    {
        User? user = await LookupUserByName(credentials.Username);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password");

        if (user.AccountLocked)
            throw new UnauthorizedAccessException("User account has been locked");

        if (!await _userManager.CheckPasswordAsync(user, credentials.Password))
        {
            await _userManager.AccessFailedAsync(user);
            throw new UnauthorizedAccessException("Invalid username or password");
        }
        else
        {
            ClaimsIdentity output = new("cookies");
            output.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()));
            output.AddClaim(new Claim(ClaimTypes.Name, user.Username));

            await _userManager.ResetAccessFailedCountAsync(user);

            return output;
        }

    }

    /// <summary>
    /// Attempts to register a user, if they do not already exist
    /// </summary>
    /// <param name="credentials">Username and password for the new user</param>
    /// <returns>List of errors that occurred while creating the user</returns>
    public async Task<List<string>> RegisterUser(Credential credentials)
    {
        if (!_features.UserRegistration)
            throw new MethodAccessException($"Feature {nameof(_features.UserRegistration)} is not enabled. Unable to call {nameof(UserService)}.{nameof(RegisterUser)}");

        List<string> errors = new();

        // First check if the user already exists
        User user = await _userManager.FindByNameAsync(credentials.Username);

        if (user == null)
        {
            // User does not exist, so create a new one
            user = new()
            {
                ID = Guid.NewGuid(),
                Username = credentials.Username
            };

            // Save to DB
            IdentityResult result = await _userManager.CreateAsync(user, credentials.Password);

            // Validate the creation worked as expected
            if (!result.Succeeded)
                errors.AddRange(result.Errors.Select(x => x.Description));
        }
        else
        {
            // User was found in the DB, add an error
            errors.Add("A user with that name already exists.");
        }

        return errors;
    }

    /// <summary>
    /// Converts the ClaimsPrincipal user to a MystiickWeb...User
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<User> GetCurrentUser(ClaimsPrincipal user) =>  Task.FromResult(new User(user));

    public async Task UpdateUsername(Credential credentials, string newUsername)
    {
        if ((await AuthenticateUser(credentials)).IsAuthenticated)
        {
            var user = await _userManager.FindByNameAsync(credentials.Username);
            user.Username = newUsername;

            await _userManager.UpdateAsync(user);
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }
    }

    public async Task UpdatePassword(Credential oldPassword, Credential newPassword)
    {
        if (newPassword.Password != newPassword.ConfirmPassword)
            throw new UnauthorizedAccessException("Password and Confirm Password must match.");

        if ((await AuthenticateUser(oldPassword)).IsAuthenticated)
        {
            var user = await _userManager.FindByNameAsync(oldPassword.Username);
            await _userManager.ChangePasswordAsync(user, oldPassword.Password, newPassword.Password);
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }
    }

    public async Task SignIn(Credential credentials)
    {
        ClaimsIdentity identity = await AuthenticateUser(credentials);
        await _contextAccessor.HttpContext.SignInAsync("cookies", new ClaimsPrincipal(identity));
    }
}
