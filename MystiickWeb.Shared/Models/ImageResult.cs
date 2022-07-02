namespace MystiickWeb.Shared.Models;

public class ImageResult
{
    public string GUID { get; init; } = string.Empty;
    public byte[] Data { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } = string.Empty;
    public string[] Tags { get; init; } = Array.Empty<string>();
    public string Category { get; init; } = string.Empty;
    public string Subcategory { get; init; } = string.Empty;
    public DateTime Created { get; init; }
    public CameraSettings Camera { get; init; } = new CameraSettings();
}
