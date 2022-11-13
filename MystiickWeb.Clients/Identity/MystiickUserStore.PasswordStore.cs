using Microsoft.AspNetCore.Identity;

using MystiickWeb.Shared.Models.User;


namespace MystiickWeb.Clients.Identity
{
    public partial class MystiickUserStore : IUserPasswordStore<User>
    {
        Task IUserPasswordStore<User>.SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;

            // If the user has been authenticated, they have a record in the database
            // The only reason they might not be authenticated setting the Password is if they are a registering a new user
            if (user.Authenticated)
            {
                // TODO: Update DB
            }

            return Task.CompletedTask;
        }

        Task<string> IUserPasswordStore<User>.GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        Task<bool> IUserPasswordStore<User>.HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }
    }
}
