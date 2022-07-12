﻿using MystiickWeb.Server.Clients;
using MystiickWeb.Server.Clients.Images;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Services;

public class PostService
{

    private readonly ILogger<PostService> _logger;
    private readonly PostDataClient _postDataClient;
    private readonly ImageDataClient _imageDataClient;

    public PostService(ILogger<PostService> logger, PostDataClient postDataClient, ImageDataClient imageDataClient)
    {
        _logger = logger;
        _postDataClient = postDataClient;
        _imageDataClient = imageDataClient;
    }

    public async Task<ImagePost[]> GetAllImagePosts()
    {
        ImagePost[] output = (await _postDataClient.GetAllPosts<ImagePost>("Photography"));

        foreach (ImagePost post in output)
        {
            var attachments = new List<ImageResult>();

            foreach (var id in post.AttachmentIDs)
            {
                attachments.Add(await _imageDataClient.GetImageByID(id));
            }

            post.Attachments = attachments.ToArray();
        }

        return output;
    }
}