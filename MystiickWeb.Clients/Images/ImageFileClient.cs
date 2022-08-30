using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;

using MystiickWeb.Shared;
using MystiickWeb.Shared.Models;
using MystiickWeb.Core.Interfaces.Clients;
using MystiickWeb.Core;

namespace MystiickWeb.Clients.Images;

[Injectable(typeof(IImageFileClient))]
public class ImageFileClient : IImageFileClient
{
    private readonly ILogger<ImageFileClient> _logger;

    public ImageFileClient(ILogger<ImageFileClient> logger)
    {
        _logger = logger;
    }

    public async Task<ImageResult> LoadImage(string filePath)
    {
        _logger.LogDebug("Reading file: {filePath}", filePath);

        var file = await File.ReadAllBytesAsync(filePath);

        var output = new ImageResult()
        {
            Data = file,
            ContentType = Image.DetectFormat(file).DefaultMimeType
        };

        return output;

    }
}
