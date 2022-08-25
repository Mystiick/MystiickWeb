using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Models.Posts;

namespace MystiickWeb.Core.Interfaces.Services;

public interface IPostService
{
    Task<IBasePost[]> GetAllPosts();
    Task<IBasePost[]> GetAllPosts(string postType);
    Task<IBasePost> GetPost(uint id);
}
