using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Clients.Images;

public class ImageFileClient
{
    private readonly ILogger<ImageFileClient> _logger;

    public ImageFileClient(ILogger<ImageFileClient> logger)
    {
        _logger = logger;
    }

    public async Task<ImageResult> LoadImage(string filePath)
    {
        _logger.LogInformation("Reading file: {filePath}", filePath);

        var output = new ImageResult()
        {
            Data = await File.ReadAllBytesAsync(filePath)
        };

        output.ContentType = Image.DetectFormat(output.Data).DefaultMimeType;

        return output;

    }
}
