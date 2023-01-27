using System.ComponentModel.DataAnnotations;

namespace MystiickWeb.Shared.Models.Posts;

/// <summary>
/// Basic post needed to deserialize generic posts
/// </summary>
public class BasePost
{
    public uint ID { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "The Post Type field is required.")]
    public virtual string PostType { get; set; } = "";

    [Required(AllowEmptyStrings = false, ErrorMessage = "The Post Title field is required.")]
    public string Title { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "The Post Contents field is required.")]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public List<PostAttachment> Attachments { get; set; } = new();
}