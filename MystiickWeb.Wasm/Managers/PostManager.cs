using Microsoft.JSInterop;

using MystiickWeb.Shared;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Models.Posts;

namespace MystiickWeb.Wasm.Managers;

[Injectable(typeof(PostManager))]
internal class PostManager : BaseManager
{
    public PostManager(HttpClient http, IJSRuntime js) : base(http, js)
    {
    }

    public async Task<Response<T[]>> GetPostsByType<T>(string type)
    {
        return await GetFromApiAsync<T[]>($"{ControllerConstants.Posts}?postType={type}");
    }
    public async Task<Response<T>> GetPostByID<T>(string id)
    {
        return await GetFromApiAsync<T>($"{ControllerConstants.Posts}/{id}");
    }

    public async Task<Response<TypelessPost[]>> GetTopPosts(int count)
    {
        return await GetFromApiAsync<TypelessPost[]>($"{ControllerConstants.Posts}?top={count}");
    }
}
