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

    [HttpGet("{imageGuid}")]
    public FileContentResult GetImage(string imageGuid)
    {
        var output = System.IO.File.ReadAllBytes("C:\\temp\\img\\archive\\X1-L4.jpg");
        _service.GetImageByGUID("");
        //base.Ok(

        return base.File(output, "image/jpeg");
    }

    [HttpGet("categories")]
    public async Task<ImageCategory[]> GetCategories()
    {
        return await _service.GetCategories();
    }
}
