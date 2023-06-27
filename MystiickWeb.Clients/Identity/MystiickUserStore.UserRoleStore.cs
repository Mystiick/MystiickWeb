using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity;

public partial class MystiickUserStore : IUserRoleStore<User>
{
    async Task IUserRoleStore<User>.AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        // Create user role
        await _userRoleDataClient.AddToRole(user, roleName, cancellationToken);

        // Create Claim/role association
        await _userClaimDataClient.AddClaim(user, new Claim(ClaimTypes.Role, roleName), cancellationToken);
    }

    Task IUserRoleStore<User>.RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken) => _userRoleDataClient.RemoveFromRole(user, roleName, cancellationToken);
    Task<IList<string>> IUserRoleStore<User>.GetRolesAsync(User user, CancellationToken cancellationToken) => GetRolesAsync(user, cancellationToken);
    private Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken) => _userRoleDataClient.GetRolesByUser(user, cancellationToken);

    async Task<bool> IUserRoleStore<User>.IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        // Validate Claim ID
        if ((await GetRolesAsync(user, cancellationToken)).Contains(roleName))
        {
            // Get claim from the passed in User
            UserClaim? userClaim = user.Claims.Find(x => x.ClaimType == ClaimTypes.Role && x.ClaimValue == roleName);
            if (userClaim?.ClaimValue == null)
                throw new AuthenticationException();

            // Check claim against database claim.
            // Every user role has an associated UserClaim, compare the passed in user's claim with the one in the database to validate authenticity
            return await _userClaimDataClient.IsUserClaimValid(user, userClaim, cancellationToken);
        }
        else
        {
            return false;
        }
    }

    Task<IList<User>> IUserRoleStore<User>.GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) => _userRoleDataClient.GetUsersInRole(roleName, cancellationToken);
}
