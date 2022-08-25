using Microsoft.AspNetCore.Mvc;

using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared.Models.Posts;
using Constants = MystiickWeb.Shared.Constants;

namespace MystiickWeb.Server.Controllers;

[ApiController]
[Route(Shared.Constants.ControllerConstants.Posts)]
public class PostsController : BaseController
{

    private readonly ILogger<PostsController> _logger;
    private readonly IPostService _postService;

    public PostsController(ILogger<PostsController> logger, IPostService postService) : base(logger)
    {
        _logger = logger;
        _postService = postService;
    }

    [HttpGet("")]
    public async Task<IBasePost[]> GetPosts(string? postType, string? top)
    {
        if (!string.IsNullOrWhiteSpace(postType))
        {
            switch (postType.ToLower())
            {
                case Constants.Post.PostType_Photography:
                    return await GetPostsOfType<ImagePost>(postType);

                case Constants.Post.PostType_Programming:
                    return await GetPostsOfType<ProgrammingPost>(postType);

                default:
                    throw new ArgumentException("todo");
            }
        }
        else if (int.TryParse(top, out int count) && count > 0)
        {
            return await GetTopPosts(count);
        }

        throw new ArgumentException("todo");
    }

    [HttpGet("{id}")]
    public async Task<IBasePost> GetPostByID(uint id)
    {
        return await _postService.GetPost(id);
    }

    private async Task<IBasePost[]> GetTopPosts(int count)
    {
        if (count > 10) throw new ArgumentException("Cannot get more than 10 posts at once");

        var output = (await _postService.GetAllPosts()).Take(count).ToArray();
        return output;
    }

    private async Task<T[]> GetPostsOfType<T>(string postType) where T : IBasePost, new()
    {
        T[] posts = (await _postService.GetAllPosts(postType)).Cast<T>().ToArray();

        return posts;
    }
}
