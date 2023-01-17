using System.Security.Authentication;
using Dapper;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity;

public partial class MystiickUserStore : IUserRoleStore<User>
{
    public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        // Create user role
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

        // Create Claim/role association
        await connection.ExecuteAsync(
            "insert into UserClaim (ID, UserID, ClaimType, ClaimValue, Created, Updated) values (@ID, @UserID, @ClaimType, @ClaimValue, @Created)",
            new
            {
                ID = Guid.NewGuid(),
                UserID = user.ID,
                ClaimType = roleName,
                ClaimValue = Guid.NewGuid(),
                Created = DateTime.Now
            }
        );
    }

    public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            "delete from UserClaim where UserID = @UserID AND ClaimValue = @Role",
            new { UserID = user.ID, Role = roleName }
        );

        await connection.ExecuteAsync(
            "delete from UserRole where UserID = @UserID AND Role = @Role",
            new { UserID = user.ID, Role = roleName }
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

    public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        // Validate Claim ID
        if ((await GetRolesAsync(user, cancellationToken)).Contains(roleName))
        {
            // Get claim from User
            var userClaim = user.Claims.Find(x => x.Type == roleName);
            if (userClaim?.Value == null)
                throw new AuthenticationException();

            // Check claim against database claim
            using var connection = new MySqlConnection(_connections.UserDatabase);
            await connection.OpenAsync(cancellationToken);

            return await connection.QueryFirstAsync<bool>(
                "select 1 output from UserClaim where UserID = @UserID AND ClaimID = @ClaimID",
                new { UserID = user.ID, ClaimID = userClaim.Value }
            );
        }
        else
        {
            return false;
        }
    }

    public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connections.UserDatabase);
        await connection.OpenAsync(cancellationToken);

        return (await connection.QueryAsync<User>("select * from User where ID IN (select UserID from UserRole where Role = @Role)")).ToList();
    }
}
