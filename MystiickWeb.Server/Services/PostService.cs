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

    public async Task<BasePost[]> GetAllPosts()
    {
        BasePost[] output = await _postDataClient.GetAllPosts();

        await ProcessAttachments(output);

        return output;
    }

    public async Task<ImagePost[]> GetAllImagePosts()
    {
        ImagePost[] output = await _postDataClient.GetAllPostsOfType<ImagePost>("Photography");

        foreach (ImagePost post in output)
        {
            post.Attachments = await GetImageAttachments(post);
        }

        return output;
    }

    public async Task<ImagePost> GetImagePost(int id)
    {
        ImagePost post = await _postDataClient.GetPost<ImagePost>(id);
        post.Attachments = await GetImageAttachments(post);

        return post;
    }

    private async Task ProcessAttachments(BasePost[] posts)
    {
        foreach (var post in posts)
        {
            if (post.GetType() == typeof(ImagePost) && post.AttachmentIDs.Any())
            {
                ((ImagePost)post).Attachments = await GetImageAttachments((ImagePost)post);
            }
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
