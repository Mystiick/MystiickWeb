using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Controllers;

[ApiController]
[Route(ControllerConstants.Minecraft)]
public class MinecraftController : BaseController
{
    private readonly ILogger<MinecraftController> _logger;
    private readonly IMinecraftService _service;

    public MinecraftController(ILogger<MinecraftController> logger, IMinecraftService service) : base(logger)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("")]
    public async Task<ActionResult<MinecraftServerData>> GetServerData()
    {
        var output = await _service.GetServerData();

        if (output != null)
        {
            var players = new List<MinecraftPlayer>();

            foreach (string name in output.OnlinePlayerNames)
            {
                players.Add(await _service.GetPlayerData(name));
            }

            output.OnlinePlayers = players.ToArray();

            return Ok(output);
        }
        else
        {
            return NoContent();
        }
    }

    [HttpGet("{name}")]
    public async Task<MinecraftPlayer> GetPlayerData(string name)
    {
        return await _service.GetPlayerData(name);
    }
}