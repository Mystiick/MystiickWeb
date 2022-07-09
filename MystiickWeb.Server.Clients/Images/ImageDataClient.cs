using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models;

using System.Data.Common;
using System.Linq;

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

    private const string SelectImageResultSql = @"select i.GUID, i.Category, i.Subcategory, GROUP_CONCAT(it.TagName) as 'Tags', Created, ist.*
                                                  from Image i
                                                  left join ImageTag it on it.ImageID = i.ImageID
                                                  left join ImageSettings ist on ist.ImageID = i.ImageID ";

    private const string GroupByImageResultSql = " group by i.GUID, ist.ImageSettingsID";


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
            output.Add(new ImageCategory()
            {
                Name = rec["Category"].ToString() ?? "No Category",
                Count = int.Parse(rec["Count"].ToString() ?? "0")
            });
        }

        return output.ToArray();
    }

    public async Task<ImageResult> GetImageByGuid(string guid)
    {
        const string query = SelectImageResultSql + " where GUID = @guid " + GroupByImageResultSql;

        var param = new MySqlParameter("@guid", guid);

        return (await GetImageData(query, param)).First();
    }

    public async Task<ImageResult[]> GetImagesByCategory(string category)
    {
        const string query = SelectImageResultSql + " where Category = @category " + GroupByImageResultSql;

        var param = new MySqlParameter("@category", category);

        return await GetImageData(query, param);
    }

    public async Task<ImageResult[]> GetImagesBySubcategory(string subcategory)
    {
        const string query = SelectImageResultSql + " where SubCategory = @subcategory " + GroupByImageResultSql;

        var param = new MySqlParameter("@subcategory", subcategory);

        return await GetImageData(query, param);
    }

    public async Task<ImageResult[]> GetImagesByTag(string tag)
    {
        const string query = SelectImageResultSql + " where i.ImageID in (select ImageID from ImageTag where TagName = @tag) " + GroupByImageResultSql;

        var param = new MySqlParameter("@tag", tag);

        return await GetImageData(query, param);
    }

    public async Task<ImageResult[]> GetImageData(string query, MySqlParameter param)
    {
        var output = new List<ImageResult>();

        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(query, connection);
        command.Parameters.Add(param);

        await command.PrepareAsync();
        DbDataReader reader = await command.ExecuteReaderAsync();

        foreach (DbDataRecord rec in reader)
        {
            output.Add(new ImageResult()
            {
                GUID = rec["GUID"].ToString() ?? "",
                Tags = (rec["Tags"].ToString() ?? "").Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
                Category = rec["Category"].ToString() ?? "",
                Subcategory = rec["Subcategory"].ToString() ?? "",
                Created = DateTime.Parse(rec["Created"].ToString() ?? ""),
                Camera = new CameraSettings()
                {
                    Model = rec["Model"].ToString() ?? "",
                    Flash = bool.Parse(rec["Flash"].ToString() ?? ""),
                    ISO = uint.Parse(rec["ISO"].ToString() ?? ""),
                    ShutterSpeed = rec["ShutterSpeed"].ToString() ?? "",
                    Aperature = rec["Aperature"].ToString() ?? "",
                    FocalLength = rec["FocalLength"].ToString() ?? "",
                }
            });
        }

        return output.ToArray();
    }

    /// <summary>
    /// Gets the file path of a given image by GUID
    /// </summary>
    /// <param name="guid"></param>
    public async Task<string> GetImagePathByGuid(string guid, bool thumbnail)
    {
        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"select ImagePath, ThumbnailPath, PreviewPath from Image where GUID = @guid", connection);
        command.Parameters.AddWithValue("@guid", guid);

        await command.PrepareAsync();
        DbDataReader reader = await command.ExecuteReaderAsync();

        return reader.Cast<DbDataRecord>().First()[thumbnail ? "ThumbnailPath" : "PreviewPath"].ToString() ?? "";
    }
}