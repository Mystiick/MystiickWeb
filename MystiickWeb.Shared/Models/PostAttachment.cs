using MystiickWeb.Shared.Models.Posts;

namespace MystiickWeb.Shared.Models;

public class PostAttachment
{
    public uint PostAttachmentID { get; init; }
    public uint ObjectID { get; init; }
    public AttachmentType AttachmentType { get; init; }
    public object? Content { get; set; }
}

public class PostAttachment<T> : PostAttachment where T : class, new()
{
    public new T? Content { get => (T?)base.Content; set => base.Content = value; }

    public PostAttachment(PostAttachment old)
    {
        PostAttachmentID = old.PostAttachmentID;
        ObjectID = old.ObjectID;
        AttachmentType = old.AttachmentType;
    }
}

public enum AttachmentType
{
    Link,
    Image,
}
