using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using MystiickWeb.Core.Interfaces.Clients.UserStore;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity.UserDataClients;

[Injectable(typeof(IUserLockoutDataClient))]
internal class MystiickUserLockoutDataClient : BaseUserDataClient, IUserLockoutDataClient
{
    public MystiickUserLockoutDataClient(IOptions<ConnectionStrings> configs) : base(configs) {}

    async Task<int> IUserLockoutDataClient.IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);

        await connection.ExecuteAsync("update User set FailedAttempts = FailedAttempts + 1 where ID = @ID", new { user.ID });
        return (await GetUserLockoutData(user, cancellationToken)).FailedAttempts;
    }

    async Task IUserLockoutDataClient.ResetAccessFailedCount(User user, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);

        await connection.ExecuteAsync("update User set FailedAttempts = 0 where ID = @ID", new { user.ID });
    }

    //todo: Test that this works passing in null
    async Task IUserLockoutDataClient.SetLockout(User user, DateTime? lockoutEnd, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        await connection.ExecuteAsync($"update User set LockoutEndDate = @Lockout where ID = @ID", new { user.ID, Lockout = lockoutEnd });
    }

    async Task IUserLockoutDataClient.SetLockoutEndDate(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);

        await connection.ExecuteAsync($"update User set LockoutEndDate = @Lockout where ID = @ID", new { user.ID, Lockout = lockoutEnd.Value.LocalDateTime });
    }

    Task<User> IUserLockoutDataClient.GetUserLockoutData(User user, CancellationToken cancellationToken) => GetUserLockoutData(user, cancellationToken);
    private async Task<User> GetUserLockoutData(User user, CancellationToken cancellationToken) 
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<User>("select ID, FailedAttempts, LockoutEndDate from User where ID = @ID", new { user.ID });
    }
}
