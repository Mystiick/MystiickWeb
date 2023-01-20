namespace MystiickWeb.Shared.Models.Posts;

public interface IBasePost
{
    public uint ID { get; set; }
    public string PostType { get; init; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<PostAttachment> Attachments { get; set; }
}
