using MystiickWeb.Shared.Models.Posts;

namespace MystiickWeb.Core.Interfaces.Services;

public interface IPostService
{
    Task<BasePost[]> GetAllPosts();
    Task<BasePost[]> GetAllPosts(string postType);
    Task<BasePost> GetPost(uint id);
    Task<BasePost> CreatePost(BasePost post);
}
