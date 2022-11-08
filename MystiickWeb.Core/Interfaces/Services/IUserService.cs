﻿using System.Security.Claims;

using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Interfaces.Services;

public interface IUserService
{
    Task<User?> LookupUserByName(string username);
    Task<ClaimsIdentity> AuthenticateUser(Credential credential);
    Task<List<string>> RegisterUser(Credential credentials);
    Task<User> GetCurrentUser(ClaimsPrincipal user);
    Task UpdateUsername(Credential credential, string newUsername);
}
