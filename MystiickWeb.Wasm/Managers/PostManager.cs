using Microsoft.JSInterop;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Models.Posts;
using Newtonsoft.Json;

namespace MystiickWeb.Wasm.Managers;

[Injectable(typeof(PostManager))]
internal class PostManager : BaseManager
{
    public PostManager(HttpClient http, IJSRuntime js) : base(http, js)
    {
    }

    public async Task<Response<T[]>> GetPostsByType<T>(string type) where T : BasePost
    {
        var output = await GetFromApiAsync<T[]>($"{ControllerConstants.Posts}?postType={type}");

        if (output.Value != null)
            foreach (var attachment in output.Value.SelectMany(x => x.Attachments))
                attachment.Content = MapAttachment(attachment);

        return output;
    }

    public async Task<Response<T>> GetPostByID<T>(string id) where T : BasePost
    {
        var output = await GetFromApiAsync<T>($"{ControllerConstants.Posts}/{id}");

        if (output.Value != null)
            foreach (var attachment in output.Value.Attachments)
                attachment.Content = MapAttachment(attachment);

        return output;
    }

    public async Task<Response<BasePost[]>> GetTopPosts(int count)
    {
        return await GetFromApiAsync<BasePost[]>($"{ControllerConstants.Posts}?top={count}");
    }

    public async Task<Response<BasePost>> CreatePost(BasePost post)
    {
        return await PostApiAsync<BasePost>(ControllerConstants.Posts, post);
    }

    private object MapAttachment(PostAttachment attachment)
    {
        if (attachment.Content == null)
            return new();

        return attachment.AttachmentType switch
        {
            AttachmentType.Link => JsonConvert.DeserializeObject<Link>(attachment.Content.ToString()),
            AttachmentType.Image => JsonConvert.DeserializeObject<ImageResult>(attachment.Content.ToString()),
            _ => throw new NotImplementedException(attachment.AttachmentType.ToString()),
        };
    }
}
