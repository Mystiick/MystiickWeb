using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared.Models.User;

using System.Security.Claims;

namespace MystiickWeb.Server.Controllers;

[ApiController]
[Route(Shared.Constants.ControllerConstants.Users)]
public class UserController : BaseController
{

    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService) : base(logger)
    {
        _userService = userService;
    }

    [ValidateAntiForgeryToken]
    [HttpPost("login")]
    public async Task<IActionResult> Login(Credential credentials)
    {
        if (ModelState.IsValid)
        {
            try
            {
                ClaimsIdentity identity = await _userService.AuthenticateUser(credentials);

                await HttpContext.SignInAsync("cookies", new ClaimsPrincipal(identity));
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new[] { "Invalid username or password." });
            }
        }

        return Unauthorized();
    }

    [ValidateAntiForgeryToken]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("cookies");
        return Ok();
    }

    [ValidateAntiForgeryToken]
    [HttpPost("register")]
    public async Task<ActionResult<List<string>>> Register(Credential credentials)
    {
        List<string> errors;

        if (ModelState.IsValid)
            errors = await _userService.RegisterUser(credentials);
        else
            errors = new List<string>() { "An unexpected error has occurred" };

        if (errors.Any())
            return BadRequest(errors);
        else
            return Ok();
    }

    [ValidateAntiForgeryToken]
    [Authorize]
    [HttpGet("claims")]
    public async Task<List<UserClaim>> GetUserClaims()
    {
        return await Task.FromResult(HttpContext.User.Identities.SelectMany(x => x.Claims).Select(x => new UserClaim(x)).ToList());
    }

    // TODO: This causes an issue when logging in/out [ValidateAntiForgeryToken]
    [HttpGet("current")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        var output = await _userService.GetCurrentUser(HttpContext.User);

        if (output != null)
            return Ok(output);
        else
            return NoContent();
    }

    [ValidateAntiForgeryToken]
    [Authorize]
    [HttpPut("current/password")]
    public async Task<ActionResult<string[]>> UpdatePassword(Credential[] input)
    {
        var oldPassword = input[0];
        var newPassword = input[1];

        // Done here since we don't trust the client
        oldPassword.Username = HttpContext.User.Identity?.Name ?? "";
        newPassword.Username = oldPassword.Username;

        try
        {
            await _userService.UpdatePassword(oldPassword, newPassword);
            await _userService.SignIn(newPassword);

            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new[] { ex.Message });
        }
    }

    [ValidateAntiForgeryToken]
    [Authorize]
    [HttpPut("current")]
    public async Task<ActionResult<string[]>> UpdateUsername([FromBody] Credential credential, [FromQuery] string username)
    {
        try
        {
            credential.Username = HttpContext.User.Identity?.Name ?? "";
            await _userService.UpdateUsername(credential, username);

            // Update username and sign in again
            credential.Username = username;
            await _userService.SignIn(credential);

            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new[] { ex.Message });
        }
    }
}
