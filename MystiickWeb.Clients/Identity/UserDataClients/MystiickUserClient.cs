using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using MystiickWeb.Core.Interfaces.Clients.UserStore;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity.UserDataClients;

[Injectable(typeof(IUserDataClient))]
public class MystiickUserDataClient : BaseUserDataClient, IUserDataClient
{
    public MystiickUserDataClient(IOptions<ConnectionStrings> configs) : base(configs) { }

    async Task<IdentityResult> IUserDataClient.CreateUser(User user, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        await connection.ExecuteAsync(
            "insert into User (ID, Username, NormalizedUsername, PasswordHash, Created, Updated, FailedAttempts) values (@ID, @Username, @NormalizedUsername, @PasswordHash, @Created, @Created, @FailedAttempts)",
            new
            {
                user.ID,
                user.Username,
                user.NormalizedUsername,
                user.PasswordHash,
                Created = DateTime.Now,
                FailedAttempts = 0
            }
        );

        return IdentityResult.Success;
    }

    async Task<User?> IUserDataClient.GetUserByID(string userID, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<User>("select * from User where ID = @ID", new { ID = userID });
    }

    async Task<User?> IUserDataClient.GetUserByName(string normalizedUserName, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<User>("select * from User where NormalizedUsername = @NormalizedUsername", new { NormalizedUsername = normalizedUserName });
    }

    async Task IUserDataClient.UpdateNormalizedUserName(User user, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        await connection.ExecuteAsync("update User set NormalizedUsername = @NormalizedUsername, Updated = @Updated where ID = @ID", new { user.NormalizedUsername, Updated = DateTime.Now, user.ID });
    }

    async Task IUserDataClient.UpdateUserName(User user, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        await connection.ExecuteAsync("update User set Username = @Username, Updated = @Updated where ID = @ID", new { user.Username, Updated = DateTime.Now, user.ID });
    }

    async Task IUserDataClient.UpdateUser(User user, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        await connection.ExecuteAsync(
            "update User set Username = @Username, NormalizedUsername = @NormalizedUsername, PasswordHash = @PasswordHash, Updated = @Updated where ID = @ID",
            new
            {
                user.Username,
                user.NormalizedUsername,
                user.PasswordHash,
                Updated = DateTime.Now,
                user.ID
            }
        );
    }

    async Task IUserDataClient.UpdatePassword(User user, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        await connection.ExecuteAsync("update User set PasswordHash = @PasswordHash where ID = @ID", new { user.PasswordHash, user.ID });
    }

    async Task IUserDataClient.DeleteUser(User user, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        await connection.ExecuteAsync("delete from User where ID = @ID", new { user.ID });
    }
}
