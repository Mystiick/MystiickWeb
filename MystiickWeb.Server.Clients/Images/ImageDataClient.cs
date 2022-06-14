using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models;

using System.Data.Common;

namespace MystiickWeb.Server.Clients.Images;

public class ImageDataClient
{
    private readonly ILogger<ImageDataClient> _logger;
    private readonly ConnectionStrings _configs;

    public ImageDataClient(ILogger<ImageDataClient> logger, IOptions<ConnectionStrings> configs)
    {
        _logger = logger;
        _configs = configs.Value;
    }

    /// <summary>
    /// Gets an array of all categories and a count of how many images use that category form the database.
    /// </summary>
    /// <returns></returns>
    public async Task<ImageCategory[]> GetCategories()
    {
        var output = new List<ImageCategory>();
        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"select Category, Count(*) as 'Count' from Image group by Category order by Count(*) desc", connection);

        DbDataReader reader = await command.ExecuteReaderAsync();

        foreach (DbDataRecord rec in reader)
        {
            output.Add(new ImageCategory() { 
                Name = rec["Category"].ToString() ?? "No Category", 
                Count = int.Parse(rec["Count"].ToString() ?? "0")
            });
        }

        return output.ToArray();
    }

    public async Task<ImageResult[]> GetImagesByCategory(string category)
    {
        const string query = @"select i.GUID, i.Category, i.Subcategory, GROUP_CONCAT(it.TagName) as 'Tags'
                              from Image i
                              left join ImageTag it on it.ImageID = i.ID
                              where Category = @category
                              group by i.GUID";
        var output = new List<ImageResult>();


        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@category", category);
        
        await command.PrepareAsync();
        DbDataReader reader = await command.ExecuteReaderAsync();

        foreach (DbDataRecord rec in reader)
        {
            output.Add(new ImageResult() {
                GUID = rec["GUID"].ToString() ?? "",
                Tags = (rec["Tags"].ToString() ?? "").Split(','),
                Category = rec["Category"].ToString() ?? "",
                Subcategory = rec["Subcategory"].ToString() ?? ""
            });
        }

        return output.ToArray();
    }

    /// <summary>
    /// Gets the file path of a given image by GUID
    /// </summary>
    /// <param name="guid"></param>
    public async Task<string> GetImagePathByGUID(string guid, bool thumbnail)
    {
        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"select ImagePath, ThumbnailPath from Image where GUID = @guid", connection);
        command.Parameters.AddWithValue("@guid", guid);

        await command.PrepareAsync();
        DbDataReader reader = await command.ExecuteReaderAsync();

        return reader.Cast<DbDataRecord>().First()[thumbnail ? "ThumbnailPath" : "ImagePath"].ToString() ?? "";
    }
}
