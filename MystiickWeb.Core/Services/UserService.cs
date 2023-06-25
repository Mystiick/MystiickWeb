using System.Security.Claims;
using System.Transactions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Constants;
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
    /// Gets the User object by username, omitting the password.
    /// </summary>
    /// <param name="username">Username to lookup</param>
    /// <returns>User object, or null if one does not exist with that name</returns>
    public async Task<User?> LookupUserByName(string username)
    {
        User user = await LookupUserByNameInternal(username);
        user.PasswordHash = "";

        return user;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="credentials"></param>
    /// <returns></returns>
    public async Task<ClaimsIdentity> AuthenticateUser(Credential credentials)
    {
        User? user = await LookupUserByNameInternal(credentials.Username);

        if (user.AccountLocked)
            throw new UnauthorizedAccessException("User account has been locked");

        if (!await _userManager.CheckPasswordAsync(user, credentials.Password))
        {
            await _userManager.AccessFailedAsync(user);
            throw new UnauthorizedAccessException("Invalid username or password");
        }
        else
        {
            ClaimsIdentity output = await BuildNewUser(user);
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
            user = new() { ID = Guid.NewGuid(), Username = credentials.Username };

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
    /// Gets the currently signed in user from the HttpContext
    /// </summary>
    public User GetCurrentUser() => new(_contextAccessor.HttpContext.User);

    /// <summary>
    /// Validates user credentials, and updates the username if the credentials are valid
    /// </summary>
    /// <param name="credentials">Credentials to validate</param>
    /// <param name="newUsername">Username to update to</param>
    /// <exception cref="UnauthorizedAccessException">If the user fails to authenticate, an UnauthorizedAccessException is thrown</exception>
    public async Task UpdateUsername(Credential credentials, string newUsername)
    {
        if ((await AuthenticateUser(credentials)).IsAuthenticated)
        {
            User user = await _userManager.FindByNameAsync(credentials.Username);
            user.Username = newUsername;

            await _userManager.UpdateAsync(user);
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }
    }

    /// <summary>
    /// Validates user credentials, and updates the password if the credentials are valid
    /// </summary>
    /// <param name="credentials">Credentials to validate</param>
    /// <param name="newPassword">New password to update to</param>
    /// <exception cref="UnauthorizedAccessException">If the user fails to authenticate, an UnauthorizedAccessException is thrown</exception>
    public async Task UpdatePassword(Credential oldPassword, Credential newPassword)
    {
        if (newPassword.Password != newPassword.ConfirmPassword)
            throw new UnauthorizedAccessException("Password and Confirm Password must match.");

        if ((await AuthenticateUser(oldPassword)).IsAuthenticated)
        {
            User user = await _userManager.FindByNameAsync(oldPassword.Username);
            await _userManager.ChangePasswordAsync(user, oldPassword.Password, newPassword.Password);
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }
    }

    /// <summary>
    /// Sign in the user with the given credentials
    /// </summary>
    /// <param name="credentials"></param>
    /// <exception cref="UnauthorizedAccessException">If the user fails to authenticate, an UnauthorizedAccessException is thrown</exception>
    public async Task SignIn(Credential credentials)
    {
        ClaimsIdentity identity = await AuthenticateUser(credentials);
        await _contextAccessor.HttpContext.SignInAsync(Identity.Cookies, new ClaimsPrincipal(identity));
    }

    public async Task AddRoleToUser(string userName, string role)
    {
        // TODO: Add TransactionScopeAsyncFlowOption.Enabled everywhere
        using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

        User currentUser = GetCurrentUser();
        if (currentUser.Authenticated && await _userManager.IsInRoleAsync(currentUser, UserRoles.Administrator))
        {
            User targetUser = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(targetUser, role);
        }

        scope.Complete();
    }

    private async Task<User> LookupUserByNameInternal(string username)
    {
        User user = await _userManager.FindByNameAsync(username);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password");

        return user;
    }

    private async Task<ClaimsIdentity> BuildNewUser(User user)
    {
        ClaimsIdentity output = new(Identity.Cookies);

        output.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()));
        output.AddClaim(new Claim(ClaimTypes.Name, user.Username));

        foreach (Claim claim in await _userManager.GetClaimsAsync(user))
            output.AddClaim(claim);

        return output;
    }
}
