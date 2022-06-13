using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Server.Services;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly ILogger<ImageController> _logger;
    private readonly ImageService _service;

    public ImageController(ILogger<ImageController> logger, ImageService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("{guid}")]
    public async Task<FileContentResult> GetImage(string guid, bool thumbnail = true)
    {
        ImageResult output = await _service.GetImageByGUID(guid, thumbnail);

        return base.File(output.Data, output.ContentType);
    }

    [HttpGet("categories")]
    public async Task<ImageCategory[]> GetCategories()
    {
        return await _service.GetCategories();
    }

    [HttpGet("categories/{category}")]
    public async Task<string[]> GetImagesByCategory(string category)
    {
        return await _service.GetImagesByCategory(category);
    }
}
