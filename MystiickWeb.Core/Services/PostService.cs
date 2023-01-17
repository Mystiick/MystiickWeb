using Microsoft.Extensions.Logging;
using MystiickWeb.Core.Interfaces.Clients;
using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Models.Posts;

namespace MystiickWeb.Core.Services;

[Injectable(typeof(IPostService))]
public class PostService : IPostService
{
    private readonly ILogger<PostService> _logger;
    private readonly IPostDataClient _postDataClient;
    private readonly IImageDataClient _imageDataClient;

    public PostService(ILogger<PostService> logger, IPostDataClient postDataClient, IImageDataClient imageDataClient)
    {
        _logger = logger;
        _postDataClient = postDataClient;
        _imageDataClient = imageDataClient;
    }

    public async Task<IBasePost[]> GetAllPosts()
    {
        IBasePost[] output = await _postDataClient.GetAllPosts();
        foreach (var post in output)
        {
            await ProcessAttachments(post);
        }

        return output;
    }

    public async Task<IBasePost[]> GetAllPosts(string postType)
    {
        IBasePost[] output = await _postDataClient.GetAllPostsOfType(postType);
        foreach (IBasePost post in output)
        {
            await ProcessAttachments(post);
        }

        return output;
    }

    public async Task<IBasePost> GetPost(uint id)
    {
        IBasePost post = await _postDataClient.GetPost(id);
        await ProcessAttachments(post);

        return post;
    }

    private async Task ProcessAttachments(IBasePost post)
    {
        if (post.GetType() == typeof(ImagePost) && post.AttachmentIDs.Any())
        {
            ((ImagePost)post).Attachments = await GetImageAttachments((ImagePost)post);
        }
        else if (post.GetType() == typeof(ProgrammingPost) && post.AttachmentIDs.Any())
        {
            ((ProgrammingPost)post).Attachments = await GetLinkAttachments((ProgrammingPost)post);
        }
    }

    private async Task<ImageResult[]> GetImageAttachments(ImagePost post)
    {
        var attachments = new List<ImageResult>();

        foreach (var id in post.AttachmentIDs)
        {
            attachments.Add(await _imageDataClient.GetImageByID(id));
        }

        post.Attachments = attachments.ToArray();

        return attachments.ToArray();
    }

    private async Task<Link[]> GetLinkAttachments(ProgrammingPost post)
    {
        var attachments = new List<Link>();

        foreach (var id in post.AttachmentIDs)
        {
            attachments.Add(await _postDataClient.GetLinkByID(id));
        }

        post.Attachments = attachments.ToArray();

        return attachments.ToArray();
    }

}
