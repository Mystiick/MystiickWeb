using MystiickWeb.Shared.Models;

namespace MystiickWeb.Core.Interfaces.Clients;

public interface IImageDataClient
{
    Task<ImageCategory[]> GetCategories();
    Task<ImageResult> GetImageByGuid(string guid);
    Task<ImageResult> GetImageByID(uint id);
    Task<ImageResult[]> GetImagesByCategory(string category);
    Task<ImageResult[]> GetImagesBySubcategory(string subcategory);
    Task<ImageResult[]> GetImagesByTag(string tag);
    Task<string> GetImagePathByGuid(string guid, bool thumbnail);
    //Task<ImageResult[]> GetImageData(string query, MySqlParameter param);
}
