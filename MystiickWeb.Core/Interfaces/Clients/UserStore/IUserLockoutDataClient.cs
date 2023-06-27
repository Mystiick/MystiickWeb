using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Interfaces.Clients.UserStore;
public interface IUserLockoutDataClient
{
    Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken);
    Task ResetAccessFailedCount(User user, CancellationToken cancellationToken);
    Task SetLockout(User user, DateTime? lockoutEnd, CancellationToken cancellationToken);
    Task SetLockoutEndDate(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken);
    Task<User> GetUserLockoutData(User user, CancellationToken cancellationToken);
}
