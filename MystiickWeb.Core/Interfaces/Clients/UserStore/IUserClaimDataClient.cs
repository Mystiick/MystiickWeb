using System.Security.Claims;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Interfaces.Clients.UserStore;
public interface IUserClaimDataClient
{
    Task<IList<UserClaim>> GetClaims(User user, CancellationToken cancellationToken);
    Task AddClaim(User user, Claim claims, CancellationToken cancellationToken);
    Task RemoveClaim(User user, Claim claim, CancellationToken cancellationToken);
    Task<bool> IsUserClaimValid(User user, UserClaim claim, CancellationToken cancellationToken);
}
