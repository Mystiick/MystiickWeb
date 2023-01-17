using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity
{
    public partial class MystiickUserStore : IUserClaimStore<User>
    {
        public async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connections.UserDatabase);
            await connection.OpenAsync(cancellationToken);

            List<Claim> output = new();

            var userClaims = await connection.QueryAsync(
                "select ID, ClaimType, ClaimValue from UserClaim where UserID = @UserID",
                new { UserID = user.ID }
            );

            foreach (var claim in userClaims)
            {
                Claim temp = new(claim.ClaimType, claim.ClaimValue);
                temp.Properties.Add(ClaimConstants.ClaimID, claim.ID.ToString());

                output.Add(temp);
            }

            return output;
        }

        public async Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (Claim claim in claims)
                await AddClaim(user, claim, cancellationToken);
        }

        public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {

            foreach (Claim claim in claims)
                await RemoveClaim(user, claim, cancellationToken);
        }

        public Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task AddClaim(User user, Claim claim, CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connections.UserDatabase);
            await connection.OpenAsync(cancellationToken);

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

        private async Task RemoveClaim(User user, Claim claim, CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connections.UserDatabase);
            await connection.OpenAsync(cancellationToken);

            await connection.ExecuteAsync(
                "delete from UserClaim where ID = @ID AND UserID = @UserID",
                new
                {
                    ID = claim.Properties[ClaimConstants.ClaimID],
                    UserID = user.ID
                }
            );
        }

    }
}
