using Dapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity;

public class MystiickUserStore : IUserStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
{

    private readonly ILogger<MystiickUserStore> _logger;
    private readonly ConnectionStrings _configs;

    public MystiickUserStore(ILogger<MystiickUserStore> logger, IOptions<ConnectionStrings> configs)
    {
        _logger = logger;
        _configs = configs.Value;
    }

    #region | IUserStore<User> |

    #region | Create |

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_configs.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        var command = new MySqlCommand(@"insert into User (ID, Username, NormalizedUsername, PasswordHash, Created, Updated) values (@ID, @Username, @NormalizedUsername, @PasswordHash, @Created, @Created)", connection);
        command.Parameters.AddWithValue("@ID", user.ID);
        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@NormalizedUsername", user.NormalizedUsername);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@Created", DateTime.Now);

        await command.ExecuteNonQueryAsync(cancellationToken);

        return IdentityResult.Success;
    }

    #endregion

    #region | Read |
    public async Task<User> FindByIdAsync(string userID, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_configs.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<User>("select * from User where ID = @ID AND Deleted is null", new { ID = userID });
    }

    public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_configs.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<User>("select * from User where NormalizedUsername = @NormalizedUsername AND Deleted is null", new { NormalizedUsername = normalizedUserName });
    }

    public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUsername);
    }

    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.ID.ToString());
    }

    public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Username);
    }
    #endregion

    #region | Update |

    public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUsername = normalizedName;

        // If the user has been authenticated, they have a record in the database
        // The only reason they might not be authenticated setting the Username is if they are a registering a new user
        if (user.Authenticated)
        {
            // TODO: Update DB
        }

        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
    {
        user.Username = userName;

        // If the user has been authenticated, they have a record in the database
        // The only reason they might not be authenticated setting the Username is if they are a registering a new user
        if (user.Authenticated)
        {
            // TODO: Update DB
        }

        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_configs.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        var command = new MySqlCommand(@"update User set Username = @Username, NormalizedUsername = @NormalizedUsername, PasswordHash = @PasswordHash where ID = @ID", connection);
        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@NormalizedUsername", user.NormalizedUsername);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@ID", user.ID);

        await command.ExecuteNonQueryAsync(cancellationToken);

        return IdentityResult.Success;
    }
    #endregion
    
    #region | Delete |
    public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    #endregion

    #endregion

    #region | IUserPasswordStore<User> |
    public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;

        // If the user has been authenticated, they have a record in the database
        // The only reason they might not be authenticated setting the Password is if they are a registering a new user
        if (user.Authenticated)
        {
            // TODO: Update DB
        }

        return Task.CompletedTask;
    }

    public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
    }
    #endregion

    #region | IUserRoleStore<User> |

    public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
