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
    //private readonly Dictionary<AttachmentType, Func<uint, Task<IAttachment>>> _attachmentLookup;

    public PostService(ILogger<PostService> logger, IPostDataClient postDataClient, IImageDataClient imageDataClient)
    {
        _logger = logger;
        _postDataClient = postDataClient;
        _imageDataClient = imageDataClient;

        //_attachmentLookup = new Dictionary<AttachmentType, Func<uint, Task<IAttachment>>>
        //{
        //    { AttachmentType.Link, _postDataClient.GetAttachmentByID },
        //    { AttachmentType.Image, _imageDataClient.GetAttachmentByID }
        //};
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

    //public async Task CreatePost<T>(T post) where T : IBasePost
    //{

    //}


    private async Task ProcessAttachments(IBasePost post)
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
