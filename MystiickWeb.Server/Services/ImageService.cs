using MystiickWeb.Server.Clients.Images;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Services;

public class ImageService
{
    private readonly ImageDataClient _client;

    public ImageService(ILogger<ImageService> logger, ImageDataClient client)
    {
        _client = client;
    }

    public object GetImageByGUID(string id)
    {
        _client.DoWork();

        return "";
    }

    public async Task<ImageCategory[]> GetCategories()
    {
        IEnumerable<ImageCategory> output = (await _client.GetCategories()).OrderByDescending(x => x.Count);

        return new[] { new ImageCategory() { Name = "Favorites" } }.Concat(output).ToArray();
    }
}
