using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Server.Services;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : Controller
{

    private readonly ILogger<PostController> _logger;
    private readonly PostService _postService;

    public PostController(ILogger<PostController> logger, PostService postService)
    {
        _logger = logger;
        _postService = postService;
    }

    [HttpGet("top/{count}")]
    public async Task<ActionResult<ImagePost[]>> GetTopPosts(int count)
    {
        // TODO: Update to a generic GetAllPosts
        return Ok((await _postService.GetAllImagePosts()).Take(count).ToArray());
    }

    [HttpGet("photography")]
    public async Task<ActionResult<ImagePost[]>> GetPhotographyPosts()
    {
        return Ok(await _postService.GetAllImagePosts());
    }
}
