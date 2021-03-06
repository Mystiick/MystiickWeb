using MystiickWeb.Server.Clients.Images;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Services;

public class ImageService
{
    private readonly ImageDataClient _dataClient;
    private readonly ImageFileClient _fileClient;

    public ImageService(ILogger<ImageService> logger, ImageDataClient dataClient, ImageFileClient fileClient)
    {
        _dataClient = dataClient;
        _fileClient = fileClient;
    }

    public async Task<ImageResult> GetImageByGuid(string guid)
    {
        // Get the image's file path
        return await _dataClient.GetImageByGuid(guid);
    }

    public async Task<ImageResult> GetImageFileByGuid(string id, bool thumbnail)
    {
        // Get the image's file path
        string path = await _dataClient.GetImagePathByGuid(id, thumbnail);

        // Load image from the path above
        return await _fileClient.LoadImage(path);
    }

    /// <summary>
    /// Gets a list of categories from the database, and prepends "Favorites" to it. Sorts by most popular category
    /// </summary>
    /// <returns></returns>
    public async Task<ImageCategory[]> GetCategories()
    {
        IEnumerable<ImageCategory> output = (await _dataClient.GetCategories()).OrderByDescending(x => x.Count);

        return new[] { new ImageCategory() { Name = "Favorites" } }.Concat(output).ToArray();
    }

    public async Task<ImageResult[]> GetImagesByCategory(string category)
    {
        return await _dataClient.GetImagesByCategory(category);
    }

    public async Task<ImageResult[]> GetImagesBySubcategory(string subcategory)
    {
        return await _dataClient.GetImagesBySubcategory(subcategory);
    }

    public async Task<ImageResult[]> GetImagesByTag(string tag)
    {
        return await _dataClient.GetImagesByTag(tag);
    }
}
