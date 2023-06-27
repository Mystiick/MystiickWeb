using MystiickWeb.Core.Interfaces.Clients;
using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Models.Posts;

namespace MystiickWeb.Core.Services;

[Injectable(typeof(IPostService))]
public class PostService : IPostService
{
    private readonly IPostDataClient _postDataClient;
    private readonly IImageDataClient _imageDataClient;

    public PostService(IPostDataClient postDataClient, IImageDataClient imageDataClient)
    {
        _postDataClient = postDataClient;
        _imageDataClient = imageDataClient;
    }

    async Task<BasePost[]> IPostService.GetAllPosts()
    {
        BasePost[] output = await _postDataClient.GetAllPosts();
        foreach (var post in output)
        {
            await ProcessAttachments(post);
        }

        return output;
    }

    async Task<BasePost[]> IPostService.GetAllPosts(string postType)
    {
        BasePost[] output = await _postDataClient.GetAllPostsOfType(postType);
        foreach (BasePost post in output)
            await ProcessAttachments(post);

        return output;
    }

    async Task<BasePost> IPostService.GetPost(uint id)
    {
        BasePost post = await _postDataClient.GetPost(id);
        await ProcessAttachments(post);

        return post;
    }

    async Task<BasePost> IPostService.CreatePost(BasePost post) => await _postDataClient.CreatePost(post);

    private async Task ProcessAttachments(BasePost post)
    {
        var postAttachments = await _postDataClient.GetPostAttachments(post.ID);
        post.Attachments = new();

        foreach (PostAttachment attachment in postAttachments)
        {
            switch (attachment.AttachmentType)
            {
                case AttachmentType.Link:
                    var link = new PostAttachment<Link>(attachment);
                    link.Content = await _postDataClient.GetLinkByID(link.ObjectID);
                    post.Attachments.Add(link);
                    break;

                case AttachmentType.Image:
                    var image = new PostAttachment<ImageResult>(attachment);
                    image.Content = await _imageDataClient.GetImageByID(image.ObjectID);
                    post.Attachments.Add(image);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
