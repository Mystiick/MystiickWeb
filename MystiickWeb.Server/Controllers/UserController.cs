﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared.Models.User;

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

    [HttpPost("login")]
    public async Task Login(Credential credentials)
    {
        Console.WriteLine(credentials.Username);
        Console.WriteLine(credentials.Password);

        await Task.CompletedTask;
    }

    //TODO: [ValidateAntiForgeryToken]
    [HttpPost("register")]
    public async Task<List<string>> Register(Credential credentials)
    {
        if (ModelState.IsValid)
            return await _userService.RegisterUser(credentials);
        else
            return new List<string>() { "An unexpected error has occurred" };
    }
}
