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

    public async Task<IBasePost[]> GetAllPosts()
    {
        var output = new List<IBasePost>();

        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(SelectPosts + GroupAndOrderPosts, connection);

        DbDataReader reader = await command.ExecuteReaderAsync();
        foreach (DbDataRecord rec in reader)
        {
            output.Add(DetermineAndPopulatePost(rec));
        }

        return output.ToArray();
    }

    public async Task<IBasePost[]> GetAllPostsOfType(string postType)
    {
        var output = new List<IBasePost>();

        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(SelectPosts + WherePostType + GroupAndOrderPosts, connection);
        command.Parameters.AddWithValue("@PostType", postType);

        DbDataReader reader = await command.ExecuteReaderAsync();
        foreach (DbDataRecord rec in reader)
        {
            output.Add(DetermineAndPopulatePost(rec));
        }

        return output.ToArray();
    }

    public async Task<IBasePost> GetPost(uint id)
    {
        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand(SelectPosts + WherePostID + GroupAndOrderPosts, connection);
        command.Parameters.AddWithValue("@ID", id);

        DbDataReader reader = await command.ExecuteReaderAsync();

        return DetermineAndPopulatePost(reader.Cast<DbDataRecord>().First());
    }

    public async Task<Link> GetLinkByID(uint id)
    {
        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();

        var command = new MySqlCommand("select * from Link where LinkID = @ID", connection);
        command.Parameters.AddWithValue ("@ID", id);

        DbDataReader reader = await command.ExecuteReaderAsync();
        DbDataRecord rec = reader.Cast<DbDataRecord>().First();

        return new Link()
        {
            ID = (uint)rec["LinkID"],
            Icon = (string)rec["Icon"],
            Text = (string)rec["LinkText"],
            Url = (string)rec["LinkUrl"]
        };
    }

    private static IBasePost DetermineAndPopulatePost(DbDataRecord rec)
    {
        return ((string)rec["PostType"]).ToLower() switch
        {
            Constants.Post.PostType_Photography => PopulatePost<ImagePost>(rec),
            Constants.Post.PostType_Programming => PopulatePost<ProgrammingPost>(rec),
            _ => throw new NotImplementedException(),
        };
    }

    private static T PopulatePost<T>(DbDataRecord rec) where T: IBasePost, new()
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
