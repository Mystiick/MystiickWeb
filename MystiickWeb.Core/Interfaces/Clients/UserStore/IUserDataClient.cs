using Microsoft.AspNetCore.Identity;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Interfaces.Clients.UserStore;

public interface IUserDataClient
{
    Task<IdentityResult> CreateUser(User user, CancellationToken cancellationToken);
    Task<User?> GetUserByID(string userID, CancellationToken cancellationToken);
    Task<User?> GetUserByName(string normalizedUserName, CancellationToken cancellationToken);
    Task UpdateNormalizedUserName(User user, CancellationToken cancellationToken);
    Task UpdateUserName(User user, CancellationToken cancellationToken);
    Task UpdateUser(User user, CancellationToken cancellationToken);
    Task UpdatePassword(User user, CancellationToken cancellationToken);
    Task DeleteUser(User user, CancellationToken cancellationToken);
}
