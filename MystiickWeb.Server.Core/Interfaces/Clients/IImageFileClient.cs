using MystiickWeb.Shared.Models;

namespace MystiickWeb.Core.Interfaces.Clients;

public interface IImageFileClient
{
    Task<ImageResult> LoadImage(string filePath);
}
