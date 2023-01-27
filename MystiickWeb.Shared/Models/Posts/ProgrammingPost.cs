namespace MystiickWeb.Shared.Models.Posts;

public class ProgrammingPost : BasePost
{
    public override string PostType { get; set; } = Constants.PostType.Programming;
}
