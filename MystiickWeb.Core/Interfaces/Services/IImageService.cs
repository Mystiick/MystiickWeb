using MystiickWeb.Shared.Models;

namespace MystiickWeb.Core.Interfaces.Services;

public interface IImageService
{
    Task<ImageResult> GetImageByGuid(string guid);
    Task<ImageResult> GetImageFileByGuid(string id, bool thumbnail);
    Task<ImageCategory[]> GetCategories();
    Task<ImageResult[]> GetImagesByCategory(string category);
    Task<ImageResult[]> GetImagesBySubcategory(string subcategory);
    Task<ImageResult[]> GetImagesByTag(string tag);
}
