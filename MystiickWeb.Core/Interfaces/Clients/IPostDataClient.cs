using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Models.Posts;

namespace MystiickWeb.Core.Interfaces.Clients;

public interface IPostDataClient
{
    Task<BasePost[]> GetAllPosts();
    Task<BasePost[]> GetAllPostsOfType(string postType);
    Task<BasePost> GetPost(uint id);
    Task<Link> GetLinkByID(uint id);
    Task<List<PostAttachment>> GetPostAttachments(uint postID);
    Task<BasePost> CreatePost(BasePost post);
}

