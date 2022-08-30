using Microsoft.JSInterop;

using MystiickWeb.Shared;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Wasm.Managers;

[Injectable(typeof(PostManager))]
internal class PostManager : BaseManager
{
    public PostManager(HttpClient http, IJSRuntime js) : base(http, js)
    {
    }

    public async Task<Response<T[]>> GetPostsByType<T>(string type)
    {
        var output = await GetFromApiAsync<T[]>($"{ControllerConstants.Posts}?postType={type}");

        return output;
    }
    public async Task<Response<T>> GetPostByID<T>(string id)
    {
        var output = await GetFromApiAsync<T>($"{ControllerConstants.Posts}/{id}");

        return output;
    }
}
