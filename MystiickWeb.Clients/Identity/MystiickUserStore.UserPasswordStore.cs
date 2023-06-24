using Microsoft.AspNetCore.Identity;
using MystiickWeb.Shared.Models.User;


namespace MystiickWeb.Clients.Identity;

public partial class MystiickUserStore : IUserPasswordStore<User>
{
    async Task IUserPasswordStore<User>.SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
    {
        // Set the hash here since the UserManager doesn't update the user in memory on its own
        user.PasswordHash = passwordHash;

        // If the user has been authenticated, they have a record in the database
        // The only reason they might not be authenticated setting the Password is if they are a registering a new user
        if (user.Authenticated)
            await _userDataClient.UpdatePassword(user, cancellationToken);
    }

    Task<string> IUserPasswordStore<User>.GetPasswordHashAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);
    Task<bool> IUserPasswordStore<User>.HasPasswordAsync(User user, CancellationToken cancellationToken) => Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
}
