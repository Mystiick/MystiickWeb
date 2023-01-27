using Dapper;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity;

public partial class MystiickUserStore : IUserLockoutStore<User>
{
    async Task<int> IUserLockoutStore<User>.GetAccessFailedCountAsync(User user, CancellationToken cancellationToken) => (await GetUserLockoutData(user, cancellationToken)).FailedAttempts;

    async Task<bool> IUserLockoutStore<User>.GetLockoutEnabledAsync(User user, CancellationToken cancellationToken) => (await GetUserLockoutData(user, cancellationToken)).AccountLocked;

    async Task<DateTimeOffset?> IUserLockoutStore<User>.GetLockoutEndDateAsync(User user, CancellationToken cancellationToken) => (await GetUserLockoutData(user, cancellationToken)).LockoutEndDate;

    async Task<int> IUserLockoutStore<User>.IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        int output;
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync("update User set FailedAttempts = FailedAttempts + 1 where ID = @ID", new { user.ID });

        output = (await GetUserLockoutData(user, cancellationToken)).FailedAttempts;

        if (output >= _identity.MaxSignInAttempts)
            await SetLockoutEnabledAsync(user, true, cancellationToken);

        return output;
    }

    async Task IUserLockoutStore<User>.ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync("update User set FailedAttempts = 0 where ID = @ID", new { user.ID });
    }

    async Task IUserLockoutStore<User>.SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken) => await SetLockoutEnabledAsync(user, enabled, cancellationToken);
    async Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        if (enabled)
            await connection.ExecuteAsync($"update User set LockoutEndDate = @Lockout where ID = @ID", new { user.ID, Lockout = DateTime.UtcNow.AddMinutes(30) });
        else
            await connection.ExecuteAsync($"update User set LockoutEndDate = NULL where ID = @ID", new { user.ID });
    }

    async Task IUserLockoutStore<User>.SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {

        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync($"update User set LockoutEndDate = @Lockout where ID = @ID", new { user.ID, Lockout = lockoutEnd.Value.LocalDateTime });
    }


    private async Task<User> GetUserLockoutData(User user, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<User>("select ID, FailedAttempts, LockoutEndDate from User where ID = @ID AND Deleted is null", new { ID = user.ID });
    }
}
