using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity;

public partial class MystiickUserStore : IUserClaimStore<User>
{
    Task<IList<Claim>> IUserClaimStore<User>.GetClaimsAsync(User user, CancellationToken cancellationToken) => GetClaimsAsync(user, cancellationToken);
    private async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
    {
        List<Claim> output = new();

        IList<UserClaim> userClaims = await _userClaimDataClient.GetClaims(user, cancellationToken);

        foreach (UserClaim claim in userClaims)
        {
            Claim temp = new(claim.ClaimType, claim.ClaimValue);
            temp.Properties.Add(ClaimConstants.ClaimID, claim.ID.ToString());

            output.Add(temp);
        }

        return output;
    }

    async Task IUserClaimStore<User>.AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (Claim claim in claims)
            await _userClaimDataClient.AddClaim(user, claim, cancellationToken);
    }

    Task IUserClaimStore<User>.ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    async Task IUserClaimStore<User>.RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (Claim claim in claims)
            await RemoveClaim(user, claim, cancellationToken);
    }

    Task<IList<User>> IUserClaimStore<User>.GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private Task RemoveClaim(User user, Claim claim, CancellationToken cancellationToken) => _userClaimDataClient.RemoveClaim(user, claim, cancellationToken);

}
