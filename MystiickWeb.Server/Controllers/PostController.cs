﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Core.Services;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models.Posts;

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

    [HttpGet]
    public async Task<BasePost[]> GetPosts(string? postType, string? top)
    {
        if (!string.IsNullOrWhiteSpace(postType))
        {
            return postType.ToLower() switch
            {
                PostType.Photography => await GetPostsOfType<ImagePost>(postType),
                PostType.Programming => await GetPostsOfType<ProgrammingPost>(postType),
                _ => throw new ArgumentException("todo"),
            };
        }
        else if (int.TryParse(top, out int count) && count > 0)
        {
            return await GetTopPosts(count);
        }

        throw new ArgumentException("todo");
    }

    [HttpGet("{id}")]
    public async Task<BasePost> GetPostByID(uint id)
    {
        return await _postService.GetPost(id);
    }


    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{UserRoles.Author},{UserRoles.Administrator}")]
    [HttpPost]
    public async Task<ActionResult<BasePost>> CreatePost(BasePost post)
    {
        BasePost output = await _postService.CreatePost(post);

        return Ok(output);
    }

    private async Task<BasePost[]> GetTopPosts(int count)
    {
        if (count > 10) throw new ArgumentException("Cannot get more than 10 posts at once");

        var output = (await _postService.GetAllPosts()).Take(count).ToArray();
        return output;
    }

    private async Task<T[]> GetPostsOfType<T>(string postType) where T : BasePost, new()
    {
        T[] posts = (await _postService.GetAllPosts(postType)).Cast<T>().ToArray();

        return posts;
    }
}
