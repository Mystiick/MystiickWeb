using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MystiickWeb.Core.Interfaces.Clients.UserStore;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity;

public partial class MystiickUserStore : IUserStore<User>
{
    private readonly IdentityConfig _identity;
    private readonly IUserDataClient _userDataClient;
    private readonly IUserClaimDataClient _userClaimDataClient;
    private readonly IUserLockoutDataClient _userLockoutDataClient;
    private readonly IUserRoleDataClient _userRoleDataClient;

    public MystiickUserStore(
        IOptions<IdentityConfig> identity,
        IUserDataClient userDataClient,
        IUserClaimDataClient userClaimDataClient,
        IUserLockoutDataClient userLockoutDataClient,
        IUserRoleDataClient userRoleDataClient
    )
    {
        _identity = identity.Value;
        _userDataClient = userDataClient;
        _userClaimDataClient = userClaimDataClient;
        _userLockoutDataClient = userLockoutDataClient;
        _userRoleDataClient = userRoleDataClient;
    }

    Task<IdentityResult> IUserStore<User>.CreateAsync(User user, CancellationToken cancellationToken) => _userDataClient.CreateUser(user, cancellationToken);


    async Task<User> IUserStore<User>.FindByIdAsync(string userID, CancellationToken cancellationToken)
    {
        User? user = await _userDataClient.GetUserByID(userID, cancellationToken);
        return await PopulateUser(user, cancellationToken);
    }

    async Task<User> IUserStore<User>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        User? user = await _userDataClient.GetUserByName(normalizedUserName, cancellationToken);
        return await PopulateUser(user, cancellationToken);
    }

    Task<string> IUserStore<User>.GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUsername);
    Task<string> IUserStore<User>.GetUserIdAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.ID.ToString());
    Task<string> IUserStore<User>.GetUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Username);


    async Task IUserStore<User>.SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUsername = normalizedName;

        // If the user has been authenticated, they have a record in the database
        // The only reason they might not be authenticated setting the Username is if they are a registering a new user. In that case, we don't want to update
        if (user.Authenticated)
            await _userDataClient.UpdateNormalizedUserName(user, cancellationToken);
    }

    async Task IUserStore<User>.SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
    {
        user.Username = userName;

        // If the user has been authenticated, they have a record in the database
        // The only reason they might not be authenticated setting the Username is if they are a registering a new user
        if (user.Authenticated)
            await _userDataClient.UpdateUserName(user, cancellationToken);
    }

    async Task<IdentityResult> IUserStore<User>.UpdateAsync(User user, CancellationToken cancellationToken)
    {
        await _userDataClient.UpdateUser(user, cancellationToken);
        return IdentityResult.Success;
    }

    async Task<IdentityResult> IUserStore<User>.DeleteAsync(User user, CancellationToken cancellationToken)
    {
        // Delete all information related to the user
        //TODO: Delete all user claims

        // Delete the user record itself
        await _userDataClient.DeleteUser(user, cancellationToken);

        return IdentityResult.Success;
    }


    private async Task<User> PopulateUser(User? user, CancellationToken cancellationToken)
    {
        #pragma warning disable CS8603 // Possible null reference return.
        // AspNetCore.Identity expects the return to be null, but doesn't expect the implementation to be `IUserStore<User?>`
        // This causes the Possible null reference return warning, so we just ignore it
        if (user == null)
            return null;
        #pragma warning restore CS8603 // Possible null reference return.

        user.Claims.AddRange((await GetClaimsAsync(user, cancellationToken)).Select(x => new UserClaim(x)));

        user.Claims.Add(new UserClaim(ClaimTypes.NameIdentifier, user.ID.ToString()));
        user.Claims.Add(new UserClaim(ClaimTypes.Name, user.Username));

        return user;
    }

    #region | IDisposable Support |
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Nothing to dispose of yet
            }

            disposedValue = true;
        }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
