using System.Security.Claims;
using MystiickWeb.Core.Interfaces.Clients.UserStore;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Models.User;
using Dapper;
using MySql.Data.MySqlClient;
using MystiickWeb.Shared.Constants;
using Microsoft.Extensions.Options;
using MystiickWeb.Shared.Configs;

namespace MystiickWeb.Clients.Identity.UserDataClients;

[Injectable(typeof(IUserClaimDataClient))]
public class MystiickUserClaimDataClient : BaseUserDataClient, IUserClaimDataClient
{
    public MystiickUserClaimDataClient(IOptions<ConnectionStrings> configs) : base(configs) {}

    async Task IUserClaimDataClient.AddClaim(User user, Claim claim, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);

        await connection.ExecuteAsync(
            "insert into UserClaim (ID, UserID, ClaimType, ClaimValue, Created, Updated) values (@ID, @UserID, @ClaimType, @ClaimValue, @Created, @Created)",
            new
            {
                ID = Guid.NewGuid(),
                UserID = user.ID,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                Created = DateTime.Now
            }
        );
    }

    async Task<IList<UserClaim>> IUserClaimDataClient.GetClaims(User user, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);

        return (await connection.QueryAsync<UserClaim>(
            "select ID, ClaimType, ClaimValue from UserClaim where UserID = @UserID",
            new { UserID = user.ID }
        )).ToList();
    }

    async Task IUserClaimDataClient.RemoveClaim(User user, Claim claim, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);

        await connection.ExecuteAsync(
            "delete from UserClaim where ID = @ID AND UserID = @UserID",
            new
            {
                ID = claim.Properties[ClaimConstants.ClaimID],
                UserID = user.ID
            }
        );
    }

    async Task<bool> IUserClaimDataClient.IsUserClaimValid(User user, UserClaim claim, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = await GetConnection(cancellationToken);
        return await connection.QueryFirstAsync<bool>(
            "select 1 output from UserClaim where UserID = @UserID AND ClaimType = @ClaimType AND ID = @ClaimID",
            new
            {
                UserID = user.ID,
                ClaimType = ClaimTypes.Role,
                ClaimID = claim.Properties["ClaimID"],
            }
        );
    }

}
