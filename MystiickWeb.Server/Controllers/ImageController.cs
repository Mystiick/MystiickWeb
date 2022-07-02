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
    public async Task<ImageResult> GetImage(string guid)
    {
        return await _service.GetImageByGuid(guid);
    }

    [HttpGet("{guid}/img")]
    public async Task<FileContentResult> GetImageFile(string guid, bool thumbnail = true)
    {
        ImageResult output = await _service.GetImageFileByGuid(guid, thumbnail);

        return base.File(output.Data, output.ContentType);
    }

    [HttpGet("categories")]
    public async Task<ImageCategory[]> GetCategories()
    {
        return await _service.GetCategories();
    }

    [HttpGet("categories/{category}")]
    public async Task<ImageResult[]> GetImagesByCategory(string category)
    {
        return await _service.GetImagesByCategory(category);
    }

    [HttpGet("subcategories/{subcategory}")]
    public async Task<ImageResult[]> GetImagesBySubcategory(string subcategory)
    {
        return await _service.GetImagesBySubcategory(subcategory);
    }

    [HttpGet("tags/{tag}")]
    public async Task<ImageResult[]> GetImagesByTag(string tag)
    {
        return await _service.GetImagesByTag(tag);
    }
}
