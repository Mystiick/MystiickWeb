using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Interfaces.Services;

public interface IUserService
{
    Task<List<string>> RegisterUser(Credential credentials);
}
