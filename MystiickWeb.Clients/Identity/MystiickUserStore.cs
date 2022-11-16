using Dapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity;

public partial class MystiickUserStore : IUserStore<User>
{
    private readonly ILogger<MystiickUserStore> _logger;
    private readonly ConnectionStrings _connections;
    private readonly IdentityConfig _identity;

    public MystiickUserStore(ILogger<MystiickUserStore> logger, IOptions<ConnectionStrings> configs, IOptions<IdentityConfig> identity)
    {
        _logger = logger;
        _connections = configs.Value;
        _identity = identity.Value;
    }

    #region | Create |
    async Task<IdentityResult> IUserStore<User>.CreateAsync(User user, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            "insert into User (ID, Username, NormalizedUsername, PasswordHash, Created, Updated) values (@ID, @Username, @NormalizedUsername, @PasswordHash, @Created, @Created)",
            new
            {
                user.ID,
                user.Username,
                user.NormalizedUsername,
                user.PasswordHash,
                Created = DateTime.Now
            }
        );

        return IdentityResult.Success;
    }
    #endregion

    #region | Read |
    async Task<User> IUserStore<User>.FindByIdAsync(string userID, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<User>("select * from User where ID = @ID AND Deleted is null", new { ID = userID });
    }

    async Task<User> IUserStore<User>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<User>("select * from User where NormalizedUsername = @NormalizedUsername AND Deleted is null", new { NormalizedUsername = normalizedUserName });
    }

    Task<string> IUserStore<User>.GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUsername);
    Task<string> IUserStore<User>.GetUserIdAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.ID.ToString());
    Task<string> IUserStore<User>.GetUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Username);
    #endregion

    #region | Update |
    async Task IUserStore<User>.SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUsername = normalizedName;

        // If the user has been authenticated, they have a record in the database
        // The only reason they might not be authenticated setting the Username is if they are a registering a new user
        if (user.Authenticated)
        {
            using var connection = new MySqlConnection(_connections.UserDatabase);
            await connection.OpenAsync(cancellationToken);

            await connection.ExecuteAsync("update User set NormalizedUsername = @NormalizedUsername, Updated = @Updated where ID = @ID", new { user.NormalizedUsername, Updated = DateTime.Now, user.ID });
        }
    }

    async Task IUserStore<User>.SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
    {
        user.Username = userName;

        // If the user has been authenticated, they have a record in the database
        // The only reason they might not be authenticated setting the Username is if they are a registering a new user
        if (user.Authenticated)
        {
            using var connection = new MySqlConnection(_connections.UserDatabase);
            await connection.OpenAsync(cancellationToken);

            await connection.ExecuteAsync("update User set Username = @Username, Updated = @Updated where ID = @ID", new { user.Username, Updated = DateTime.Now, user.ID } );
        }
    }

    async Task<IdentityResult> IUserStore<User>.UpdateAsync(User user, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        connection.Execute(
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

        return IdentityResult.Success;
    }
    #endregion

    #region | Delete |
    async Task<IdentityResult> IUserStore<User>.DeleteAsync(User user, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync("update User set Deleted = @Deleted where ID = @ID", new { Deleted = DateTime.Now, user.ID });

        return IdentityResult.Success;
    }
    #endregion

    #region | IDisposable Support |

    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Nothing to dispose of yet
            }

            disposedValue = true;
        }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
    }

    #endregion
}
