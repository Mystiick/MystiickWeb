using System.Security.Claims;

using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Interfaces.Services;

public interface IUserService
{
    Task<User?> LookupUserByName(string username);
    Task<ClaimsIdentity> AuthenticateUser(Credential credentials);
    Task<List<string>> RegisterUser(Credential credentials);
    User GetCurrentUser();
    Task UpdateUsername(Credential credentials, string newUsername);
    Task UpdatePassword(Credential oldPassword, Credential newPassword);
    Task SignIn(Credential credentials);
}
