using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Server.Services;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController
{

    private readonly ILogger<PostController> _logger;
    private readonly PostService _postService;

    public PostController(ILogger<PostController> logger, PostService postService)
    {
        _logger = logger;
        _postService = postService;
    }

    [HttpGet("photography")]
    public async Task<ImagePost[]> GetPhotographyPosts()
    {
        return await _postService.GetAllImagePosts();
    }
}
