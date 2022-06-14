namespace MystiickWeb.Shared.Models;

public class ImageResult
{
    public string GUID { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string Category { get; set; } = string.Empty;
    public string Subcategory { get; set; } = string.Empty;
}
