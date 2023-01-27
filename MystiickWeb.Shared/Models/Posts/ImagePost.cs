namespace MystiickWeb.Shared.Models.Posts;

public class ImagePost : BasePost
{
    public override string PostType { get; set; } = Constants.PostType.Photography;
}
