using Dapper;

using Microsoft.AspNetCore.Identity;

using MySql.Data.MySqlClient;

using MystiickWeb.Shared.Models.User;


namespace MystiickWeb.Clients.Identity
{
    public partial class MystiickUserStore : IUserRoleStore<User>
    {
        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connections.UserDatabase);
            await connection.OpenAsync(cancellationToken);

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

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connections.UserDatabase);
            await connection.OpenAsync(cancellationToken);

            await connection.ExecuteAsync(
                "delete from UserRole where UserID = @UserID AND Role = @Role",
                new
                {
                    UserID = user.ID,
                    Role = roleName
                }
            );
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connections.UserDatabase);
            await connection.OpenAsync(cancellationToken);

            var output = await connection.QueryAsync<string>(
                "select Role from UserRole where UserID = @UserID",
                new { UserID = user.ID }
            );

            return output.ToList();
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken) => (await GetRolesAsync(user, cancellationToken)).Contains(roleName);

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connections.UserDatabase);
            await connection.OpenAsync(cancellationToken);

            return (await connection.QueryAsync<User>("select * from User where ID IN (select UserID from UserRole where Role = @Role)")).ToList();
        }
    }
}
