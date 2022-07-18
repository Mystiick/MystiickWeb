using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

using Constants = MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.Posts;

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

    private const string SelectPosts = @"select p.PostID, p.PostTitle, p.PostText, p.Created, GROUP_CONCAT(pa.ObjectID) as AttachmentIDs, p.PostType
                                         from Post p
                                         left join PostAttachment pa on p.PostID = pa.PostID";

    private const string WherePostType = " where PostType = @PostType ";
    private const string WherePostID = " where p.PostID = @ID ";
    private const string GroupAndOrderPosts = " group by p.PostID order by Created desc " ;


    public async Task<BasePost[]> GetAllPosts()
    {
        var output = new List<BasePost>();

        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(SelectPosts + GroupAndOrderPosts, connection);

        DbDataReader reader = await command.ExecuteReaderAsync();
        foreach (DbDataRecord rec in reader)
        {
            switch ((string)rec["PostType"])
            {
                case Constants.Post.PostType_Photography:
                    output.Add(PopulatePost<ImagePost>(rec));
                    break;

                case Constants.Post.PostType_Programming:
                    output.Add(PopulatePost<ProgrammingPost>(rec));
                    break;
            }
        }

        return output.ToArray();
    }

    public async Task<T[]> GetAllPostsOfType<T>(string postType) where T : BasePost, new()
    {
        var output = new List<T>();

        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(SelectPosts + WherePostType + GroupAndOrderPosts, connection);
        command.Parameters.AddWithValue("@PostType", postType);

        DbDataReader reader = await command.ExecuteReaderAsync();
        foreach (DbDataRecord rec in reader)
        {
            output.Add(PopulatePost<T>(rec));
        }

        return output.ToArray();
    }

    public async Task<T> GetPost<T>(int id) where T : BasePost, new()
    {
        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(SelectPosts + WherePostID + GroupAndOrderPosts, connection);
        command.Parameters.AddWithValue("@ID", id);

        DbDataReader reader = await command.ExecuteReaderAsync();

        return PopulatePost<T>(reader.Cast<DbDataRecord>().First());
    }

    private static T PopulatePost<T>(DbDataRecord rec) where T: BasePost, new()
    {
        return new T()
        {
            ID = (uint)rec["PostID"],
            Title = (string)rec["PostTitle"],
            Text = ((string)rec["PostText"]).Split(new string[] { "\\n" }, StringSplitOptions.TrimEntries),
            CreatedDate = (DateTime)rec["Created"],
            AttachmentIDs = (rec["AttachmentIDs"].ToString() ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => uint.Parse(x)).ToArray()
        };
    }
}
