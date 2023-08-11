using MystiickWeb.Core.Interfaces.Clients;
using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Core.Services;
using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Models.Posts;

namespace MystiickWeb.Core.Tests.Services;

[TestClass]
public class PostServiceTests
{
    [TestMethod]
    public async Task PostServiceTests_GetPost_CanGetPost()
    {
        // Arrange
        var mockPostClient = Substitute.For<IPostDataClient>();        
        mockPostClient.GetPost(default).ReturnsForAnyArgs(Task.FromResult((BasePost)new ImagePost()));
        mockPostClient.GetPostAttachments(default).ReturnsForAnyArgs(Task.FromResult(new List<PostAttachment>()));
        mockPostClient.GetLinkByID(default).ReturnsForAnyArgs(Task.FromResult(new Link()));

        var mockImageClient = Substitute.For<IImageDataClient>();
        mockImageClient.GetImageByID(default).ReturnsForAnyArgs(Task.FromResult(new ImageResult()));

        IPostService postService = new PostService(mockPostClient, mockImageClient);

        // Act
        BasePost unit = await postService.GetPost(123);

        // Assert
        Assert.IsNotNull(unit);
        Assert.IsNotNull(unit.Attachments);
        Assert.AreEqual(0, unit.Attachments.Count);
    }

    [TestMethod]
    public async Task PostServiceTests_GetPost_CanProcessAttachments()
    {
        // Arrange
        var mockPostClient = Substitute.For<IPostDataClient>();
        mockPostClient.GetPost(default).ReturnsForAnyArgs(Task.FromResult((BasePost)new ImagePost()));
        mockPostClient.GetPostAttachments(default).ReturnsForAnyArgs(Task.FromResult(
            new List<PostAttachment>()
            {
                new PostAttachment() { AttachmentType = AttachmentType.Image },
                new PostAttachment() { AttachmentType = AttachmentType.Link },
            }
        ));
        mockPostClient.GetLinkByID(default).ReturnsForAnyArgs(Task.FromResult(new Link()));

        var mockImageClient = Substitute.For<IImageDataClient>();
        mockImageClient.GetImageByID(default).ReturnsForAnyArgs(Task.FromResult(new ImageResult()));

        IPostService postService = new PostService(mockPostClient, mockImageClient);

        // Act
        BasePost unit = await postService.GetPost(123);

        // Assert
        Assert.IsNotNull(unit);
        Assert.IsNotNull(unit.Attachments);
        Assert.AreEqual(2, unit.Attachments.Count);

        Assert.IsInstanceOfType(unit.Attachments[0], typeof(PostAttachment<ImageResult>));
        Assert.IsInstanceOfType(unit.Attachments[0].Content, typeof(ImageResult));

        Assert.IsInstanceOfType(unit.Attachments[1], typeof(PostAttachment<Link>));
        Assert.IsInstanceOfType(unit.Attachments[1].Content, typeof(Link));
    }
}
