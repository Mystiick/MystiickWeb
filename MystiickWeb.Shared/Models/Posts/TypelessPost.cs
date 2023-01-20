namespace MystiickWeb.Shared.Models.Posts;

/// <summary>
/// Basic post needed to deserialize generic posts
/// </summary>
public class TypelessPost : IBasePost
{
    public uint ID { get; set; }
    public string PostType { get; init; } = "";
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public List<PostAttachment> Attachments { get; set; } = new();
}