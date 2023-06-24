using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Interfaces.Clients.UserStore;

public interface IUserRoleDataClient
{
    Task AddToRole(User user, string roleName, CancellationToken cancellationToken);
    Task RemoveFromRole(User user, string roleName, CancellationToken cancellationToken);
    Task<IList<string>> GetRolesByUser(User user, CancellationToken cancellationToken);
    Task<IList<User>> GetUsersInRole(string roleName, CancellationToken cancellationToken);
}
