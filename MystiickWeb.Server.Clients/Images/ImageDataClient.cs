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

    public void DoWork()
    {
        _logger.LogInformation("ImageDataClient.DoWork() Called");
        _logger.LogInformation(_configs.ImageDatabase);
    }

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
}
