namespace MystiickWeb.Shared.Models;

public abstract class BasePost
{
    public uint ID { get; set; }
    public abstract string PostType { get; init; }
    public string Title { get; set; } = string.Empty;
    public string[] Text { get; set; } = Array.Empty<string>();
    public DateTime CreatedDate { get; set; }
    public uint[] AttachmentIDs { get; set; } = Array.Empty<uint>();
}
