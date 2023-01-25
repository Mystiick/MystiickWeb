﻿using System.Data.Common;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using MystiickWeb.Core.Interfaces.Clients;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.Posts;
using Constants = MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Clients;

[Injectable(typeof(IPostDataClient))]
public class PostDataClient : IPostDataClient
{
    private readonly ILogger<PostDataClient> _logger;
    private readonly ConnectionStrings _configs;

    public PostDataClient(ILogger<PostDataClient> logger, IOptions<ConnectionStrings> configs)
    {
        _logger = logger;
        _configs = configs.Value;
    }

    private const string SelectPosts = "select * from Post p";
    private const string WherePostType = " where PostType = @PostType ";
    private const string WherePostID = " where PostID = @ID ";
    private const string GroupAndOrderPosts = " group by PostID order by Created desc ";

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
        command.Parameters.AddWithValue("@ID", id);

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

    public async Task<List<PostAttachment>> GetPostAttachments(uint postID)
    {
        using var connection = new MySqlConnection(_configs.ImageDatabase);
        await connection.OpenAsync();
        var results = await connection.QueryAsync<PostAttachment>("select * from PostAttachment where PostID = @PostID", new { PostID = postID });

        return results.ToList();
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

    private static T PopulatePost<T>(DbDataRecord rec) where T : IBasePost, new()
    {
        return new T()
        {
            ID = (uint)rec["PostID"],
            Title = (string)rec["PostTitle"],
            Text = (string)rec["PostText"],
            CreatedDate = (DateTime)rec["Created"]
        };
    }

    //async public Task<IAttachment> GetAttachmentByID(uint id) => (IAttachment)(await GetLinkByID(id));
}
