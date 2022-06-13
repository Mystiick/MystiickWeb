namespace MystiickWeb.Shared.Models;

public class ImageResult
{
    public string ID { get; set; } = "";
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "";
}
