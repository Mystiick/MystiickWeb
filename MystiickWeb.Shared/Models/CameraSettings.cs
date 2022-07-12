namespace MystiickWeb.Shared.Models;

public class CameraSettings
{
    public string Model { get; init; } = string.Empty;
    public bool Flash { get; init; }
    public short ISO { get; init; }
    public string ShutterSpeed { get; init; } = string.Empty;
    public string Aperature { get; init; } = string.Empty;
    public string FocalLength { get; init; } = string.Empty;
}