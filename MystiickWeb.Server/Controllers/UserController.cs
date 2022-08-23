using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Server.Controllers;

[ApiController]
[Route(Shared.Constants.ControllerConstants.Users)]
public class UserController : BaseController
{
    public UserController(ILogger<UserController> logger) : base(logger)
    {
    }

    [HttpPost("login")]
    public async Task Login(Credential credentials)
    {
        Console.WriteLine(credentials.Username);
        Console.WriteLine(credentials.Password);

        await Task.CompletedTask;
    }

}
