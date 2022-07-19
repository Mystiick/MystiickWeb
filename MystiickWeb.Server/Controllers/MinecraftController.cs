using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Server.Services;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Controllers;

[ApiController]
[Route(ControllerConstants.Minecraft)]
public class MinecraftController : ControllerBase
{
    private readonly ILogger<MinecraftController> _logger;
    private readonly MinecraftService _service;

    public MinecraftController(ILogger<MinecraftController> logger, MinecraftService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("")]
    public async Task<MinecraftServerData> GetPlayers()
    {
        var output = await _service.GetServerData();
        var players = new List<MinecraftPlayer>();

        foreach(string name in output.OnlinePlayerNames)
        {
            players.Add(await _service.GetPlayerData(name));
        }

        output.OnlinePlayers = players.ToArray();

        return output;
    }

    [HttpGet("{name}")]
    public async Task<MinecraftPlayer> GetPlayerData(string name)
    {
        return await _service.GetPlayerData(name);
    }
}