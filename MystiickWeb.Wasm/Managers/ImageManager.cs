using Microsoft.JSInterop;

using MystiickWeb.Shared;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Wasm.Managers;

[Injectable(typeof(ImageManager))]
internal class ImageManager : BaseManager
{
    public ImageManager(HttpClient http, IJSRuntime js) : base(http, js) { }

    public async Task<Response<ImageResult>> GetImageByID(string guid)
    {
        return await GetFromApiAsync<ImageResult>($"{ControllerConstants.Images}/{guid}");
    }
    public async Task<Response<ImageCategory[]>> GetCategories()
    {
        return await GetFromApiAsync<ImageCategory[]>($"{ControllerConstants.Images}/categories");
    }

    public async Task<Response<ImageResult[]>> GetImagesByCategory(string category)
    {
        return await GetFromApiAsync<ImageResult[]>($"{ControllerConstants.Images}/categories/{category}");
    }

    public async Task<Response<ImageResult[]>> GetImagesBySubategory(string subcategory)
    {
        return await GetFromApiAsync<ImageResult[]>($"{ControllerConstants.Images}/subcategories/{subcategory}");
    }

    public async Task<Response<ImageResult[]>> GetImagesByTag(string tag)
    {
        return await GetFromApiAsync<ImageResult[]>($"{ControllerConstants.Images}/tags/{tag}");
    }
}
