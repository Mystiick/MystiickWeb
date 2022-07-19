namespace MystiickWeb.Shared.Models.Posts;

public class ProgrammingPost : IBasePost
{
    public uint ID { get; set; }
    public string PostType { get; init; } = Constants.Post.PostType_Programming;
    public string Title { get; set; } = string.Empty;
    public string[] Text { get; set; } = Array.Empty<string>();
    public DateTime CreatedDate { get; set; }
    public uint[] AttachmentIDs { get; set; } = Array.Empty<uint>();
}
