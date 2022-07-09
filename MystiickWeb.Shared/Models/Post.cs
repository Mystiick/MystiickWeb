
namespace MystiickWeb.Shared.Models;

public class Post<T>
{
    public int ID { get; set; }
    //public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string[] Text { get; set; } = Array.Empty<string>();
    public DateTime CreatedDate { get; set; }
    public Attachment<T>[] Attachments { get; set; } = Array.Empty<Attachment<T>>();
}
