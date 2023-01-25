namespace MystiickWeb.Shared.Models.Posts;

public class ProgrammingPost : IBasePost
{
    public uint ID { get; set; }
    public string PostType { get; init; } = Constants.PostType.Programming;
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public uint[] AttachmentIDs { get; set; } = Array.Empty<uint>();
    //public Link[] Attachments { get; set; } = Array.Empty<Link>();
    public List<PostAttachment> Attachments { get; set; } = new();
}
