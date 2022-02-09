using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Server.Services;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MinecraftController : ControllerBase
{
    private readonly ILogger<MinecraftController> _logger;
    private readonly MinecraftService _service;

    public MinecraftController(ILogger<MinecraftController> logger, MinecraftService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<MinecraftServerData> Get()
    {
        return await _service.GetServerData();
    }
}