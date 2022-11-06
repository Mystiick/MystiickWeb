﻿using Microsoft.AspNetCore.Authentication;
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
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
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
    public async Task<ActionResult> UpdatePassword((Credential currentUser, Credential newPassword) input)
    {
        await Task.CompletedTask;
        return Ok();
    }

    [ValidateAntiForgeryToken]
    [Authorize]
    [HttpPut("current")]
    public async Task<ActionResult> UpdateUsername(Credential user, [FromQuery] string newUsername)
    {
        await Task.CompletedTask;
        return Ok();
    }
}
