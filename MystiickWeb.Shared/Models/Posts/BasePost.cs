namespace MystiickWeb.Shared.Models;

public class BasePost
{
    public uint ID { get; set; }
    public virtual string PostType { get; init; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string[] Text { get; set; } = Array.Empty<string>();
    public DateTime CreatedDate { get; set; }
    public uint[] AttachmentIDs { get; set; } = Array.Empty<uint>();
}
