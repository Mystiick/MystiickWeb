using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models;

using System.Data.Common;

namespace MystiickWeb.Server.Clients;

public class PostDataClient
{

    private readonly ILogger<PostDataClient> _logger;
    private readonly ConnectionStrings _configs;

    public PostDataClient(ILogger<PostDataClient> logger, IOptions<ConnectionStrings> configs)
    {
        _logger = logger;
        _configs = configs.Value;
    }

    private const string SelectPosts = @"select p.PostID, p.PostTitle, p.PostText, p.Created, GROUP_CONCAT(pa.ObjectID) as AttachmentIDs
                                         from Post p
                                         join PostAttachment pa on p.PostID = pa.PostID
                                         where PostType = 'photography'
                                         group by p.PostID
                                         order by Created desc";

    public async Task<T[]> GetAllPosts<T>(string postType) where T : BasePost, new()
    {
        var output = new List<T>();

        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(SelectPosts, connection);
        command.Parameters.AddWithValue("@PostType", postType);

        DbDataReader reader = await command.ExecuteReaderAsync();
        foreach (DbDataRecord rec in reader)
        {            
            output.Add(new T()
            {
                ID = (uint)rec["PostID"],
                Title = (string)rec["PostTitle"],
                Text = ((string)rec["PostText"]).Split(new string[] { "\\n" }, StringSplitOptions.TrimEntries),
                CreatedDate = (DateTime)rec["Created"],
                AttachmentIDs = ((string)rec["AttachmentIDs"]).Split(',').Select(x => uint.Parse(x)).ToArray()
            });
        }

        return output.ToArray();
    }
}
