using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Api.Controllers;

[ApiController]
[Route(Shared.Constants.ControllerConstants.Images)]
public class ImagesController : BaseController
{
    private readonly IImageService _service;

    public ImagesController(ILogger<ImagesController> logger, IImageService service) : base(logger)
    {
        _service = service;
    }

    [HttpGet("{guid}")]
    public async Task<ImageResult> GetImage(string guid) => await _service.GetImageByGuid(guid);

    [HttpGet("{guid}/img")]
    public async Task<FileContentResult> GetImageFile(string guid, bool thumbnail = true)
    {
        ImageResult output = await _service.GetImageFileByGuid(guid, thumbnail);

        return base.File(output.Data, output.ContentType);
    }

    [HttpGet("categories")]
    public Task<ImageCategory[]> GetCategories() => _service.GetCategories();

    [HttpGet("categories/{category}")]
    public Task<ImageResult[]> GetImagesByCategory(string category) => _service.GetImagesByCategory(category);

    [HttpGet("subcategories/{subcategory}")]
    public Task<ImageResult[]> GetImagesBySubcategory(string subcategory) => _service.GetImagesBySubcategory(subcategory);

    [HttpGet("tags/{tag}")]
    public Task<ImageResult[]> GetImagesByTag(string tag) => _service.GetImagesByTag(tag);
}
