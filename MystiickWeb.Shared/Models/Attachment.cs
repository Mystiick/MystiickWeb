namespace MystiickWeb.Shared.Models;

public class Attachment<T>
{
    public int ID { get; set; }
    public int RelatedID { get; set; }
    public T? RelatedObject { get; set; }
}
