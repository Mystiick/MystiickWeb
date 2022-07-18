using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Server.Services;
using MystiickWeb.Shared.Models.Posts;

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
        return Ok((await _postService.GetAllPosts()).Take(count).ToArray());
    }

    [HttpGet("photography")]
    public async Task<ActionResult<ImagePost[]>> GetPhotographyPosts()
    {
        return Ok(await _postService.GetAllImagePosts());
    }

    [HttpGet("photography/{id}")]
    public async Task<ActionResult<ImagePost[]>> GetPhotographyPostByID(int id)
    {
        return Ok(await _postService.GetImagePost(id));
    }
}
