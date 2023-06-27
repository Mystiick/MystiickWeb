using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using MystiickWeb.Shared.Configs;

namespace MystiickWeb.Clients.Identity.UserDataClients;
public class BaseUserDataClient
{
    protected readonly ConnectionStrings _connections;
    public BaseUserDataClient(IOptions<ConnectionStrings> configs)
    {
        _connections = configs.Value;
    }

    /// <summary>
    /// Creates and opens a MySqlConnection object using the <see cref="ConnectionStrings.UserDatabase "/> configuration
    /// </summary>
    /// <remarks>
    /// Ensure you always dispose of this connection
    /// </remarks>
    protected async Task<MySqlConnection> GetConnection(CancellationToken cancellationToken)
    {
        var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}
