using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using MystiickWeb.Core.Interfaces.Clients.UserStore;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity.UserDataClients;

[Injectable(typeof(IUserRoleDataClient))]
public class MystiickUserRoleDataClient : BaseUserDataClient, IUserRoleDataClient
{
    public MystiickUserRoleDataClient(IOptions<ConnectionStrings> configs) : base(configs) {}

    async Task IUserRoleDataClient.AddToRole(User user, string roleName, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        await connection.ExecuteAsync(
            "insert into UserRole (ID, UserID, Role, Created, Updated) values (@ID, @UserID, @Role, @Created, @Created)",
            new
            {
                ID = Guid.NewGuid(),
                UserID = user.ID,
                Role = roleName,
                Created = DateTime.Now
            }
        );
    }

    async Task<IList<string>> IUserRoleDataClient.GetRolesByUser(User user, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        var output = await connection.QueryAsync<string>(
            "select Role from UserRole where UserID = @UserID",
            new { UserID = user.ID }
        );

        return output.Select(x => x.ToUpper()).ToList();
    }

    async Task<IList<User>> IUserRoleDataClient.GetUsersInRole(string roleName, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        return (await connection.QueryAsync<User>("select * from User where ID IN (select UserID from UserRole where Role = @Role)")).ToList();
    }

    async Task IUserRoleDataClient.RemoveFromRole(User user, string roleName, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);

        await connection.ExecuteAsync(
            "delete from UserClaim where UserID = @UserID AND ClaimValue = @Role",
            new { UserID = user.ID, Role = roleName }
        );

        await connection.ExecuteAsync(
            "delete from UserRole where UserID = @UserID AND Role = @Role",
            new { UserID = user.ID, Role = roleName }
        );
    }
}
