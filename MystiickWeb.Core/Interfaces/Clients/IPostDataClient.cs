using MystiickWeb.Shared.Models.Posts;

namespace MystiickWeb.Core.Interfaces.Clients;

public interface IPostDataClient
{
    Task<IBasePost[]> GetAllPosts();
    Task<IBasePost[]> GetAllPostsOfType(string postType);
    Task<IBasePost> GetPost(uint id);
    Task<Link> GetLinkByID(uint id);
}

