using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

using MystiickWeb.Core.Interfaces.Clients;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models;

using System.Data.Common;
using System.Linq;

namespace MystiickWeb.Server.Clients.Images;

public class ImageDataClient : IImageDataClient
{
    private readonly ILogger<ImageDataClient> _logger;
    private readonly ConnectionStrings _configs;

    public ImageDataClient(ILogger<ImageDataClient> logger, IOptions<ConnectionStrings> configs)
    {
        _logger = logger;
        _configs = configs.Value;
    }

    private const string SelectImageResultSql = @"select i.ImageID, i.GUID, i.Category, i.Subcategory, GROUP_CONCAT(it.TagName) as 'Tags', Created, ist.*
                                                  from Image i
                                                  left join ImageTag it on it.ImageID = i.ImageID
                                                  left join ImageSettings ist on ist.ImageID = i.ImageID ";

    private const string GroupByImageResultSql = " group by i.GUID, ist.ImageSettingsID ";
    private const string OrderByImageResultSql = " order by i.Created desc ";


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
                Count = (long)rec["Count"]
            });
        }

        return output.ToArray();
    }

    public async Task<ImageResult> GetImageByGuid(string guid)
    {
        const string query = SelectImageResultSql + " where GUID = @guid " + GroupByImageResultSql + OrderByImageResultSql;

        var param = new MySqlParameter("@guid", guid);

        return (await GetImageData(query, param)).First();
    }

    public async Task<ImageResult> GetImageByID(uint id)
    {
        const string query = SelectImageResultSql + " where i.ImageID = @id " + GroupByImageResultSql + OrderByImageResultSql;

        var param = new MySqlParameter("@id", id);

        return (await GetImageData(query, param)).First();
    }

    public async Task<ImageResult[]> GetImagesByCategory(string category)
    {
        const string query = SelectImageResultSql + " where Category = @category " + GroupByImageResultSql + OrderByImageResultSql;

        var param = new MySqlParameter("@category", category);

        return await GetImageData(query, param);
    }

    public async Task<ImageResult[]> GetImagesBySubcategory(string subcategory)
    {
        const string query = SelectImageResultSql + " where SubCategory = @subcategory " + GroupByImageResultSql + OrderByImageResultSql;

        var param = new MySqlParameter("@subcategory", subcategory);

        return await GetImageData(query, param);
    }

    public async Task<ImageResult[]> GetImagesByTag(string tag)
    {
        const string query = SelectImageResultSql + " where i.ImageID in (select ImageID from ImageTag where TagName = @tag) " + GroupByImageResultSql + OrderByImageResultSql;

        var param = new MySqlParameter("@tag", tag);

        return await GetImageData(query, param);
    }

    private async Task<ImageResult[]> GetImageData(string query, MySqlParameter param)
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
                ID = (uint)rec["ImageID"],
                GUID = (Guid)rec["GUID"],
                Tags = (rec["Tags"].ToString() ?? "").Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
                Category = (string)rec["Category"],
                Subcategory = (string)rec["Subcategory"],
                Created = (DateTime)rec["Created"],
                Camera = new CameraSettings()
                {
                    Model = (string)rec["Model"],
                    Flash = (bool)rec["Flash"],
                    ISO = (short)rec["ISO"],
                    ShutterSpeed = (string)rec["ShutterSpeed"],
                    Aperature = (string)rec["Aperature"],
                    FocalLength = (string)rec["FocalLength"],
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

        return (string)reader.Cast<DbDataRecord>().First()[thumbnail ? "ThumbnailPath" : "PreviewPath"];
    }
}