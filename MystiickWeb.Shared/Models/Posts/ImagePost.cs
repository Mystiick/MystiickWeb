namespace MystiickWeb.Shared.Models;

public class ImagePost : BasePost
{
    public ImageResult[] Attachments { get; set; } = Array.Empty<ImageResult>();
}