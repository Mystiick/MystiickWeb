using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Services;

[Injectable(typeof(IUserService))]
public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly UserManager<User> _userManager;

    public UserService(ILogger<UserService> logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    /// <summary>
    /// Attempts to register a user, if they do not already exist
    /// </summary>
    /// <param name="credentials">Username and password for the new user</param>
    /// <returns>List of errors that occurred while creating the user</returns>
    public async Task<List<string>> RegisterUser(Credential credentials)
    {
        List<string> errors = new();


        // First check if the user already exists
        var user = await _userManager.FindByNameAsync(credentials.Username);

        if (user == null)
        {
            // User does not exist, so create a new one
            user = new User()
            {
                ID = Guid.NewGuid(),
                Username = credentials.Username
            };

            // Save to DB
            IdentityResult result = await _userManager.CreateAsync(user, credentials.Password);

            // Validate the creation worked as expected
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
            }
        }
        else
        {
            // User was found in the DB, add an error
            errors.Add("A user with that name already exists.");
        }

        return errors;
    }
}
