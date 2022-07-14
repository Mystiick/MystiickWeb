namespace MystiickWeb.Shared.Models;

public class ImagePost : BasePost
{
    public override string PostType { get; init; } = "Photography";
    public ImageResult[] Attachments { get; set; } = Array.Empty<ImageResult>();
}