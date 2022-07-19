using MystiickWeb.Server.Clients;
using MystiickWeb.Server.Clients.Images;
using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Models.Posts;

namespace MystiickWeb.Server.Services;

public class PostService
{
    private readonly ILogger<PostService> _logger;
    private readonly PostDataClient _postDataClient;
    private readonly ImageDataClient _imageDataClient;

    public PostService(ILogger<PostService> logger, PostDataClient postDataClient, ImageDataClient imageDataClient)
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

    public async Task<IBasePost> GetPost(int id)
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

}
