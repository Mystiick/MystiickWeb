using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        Mock<IPostDataClient> mockPostClient = new();
        mockPostClient.Setup(x => x.GetPost(It.IsAny<uint>())).Returns(Task.FromResult((BasePost)new ImagePost()));
        mockPostClient.Setup(x => x.GetPostAttachments(It.IsAny<uint>())).Returns(Task.FromResult(new List<PostAttachment>()));
        mockPostClient.Setup(x => x.GetLinkByID(It.IsAny<uint>())).Returns(Task.FromResult(new Link()));

        Mock<IImageDataClient> mockImageClient = new();
        mockImageClient.Setup(x => x.GetImageByID(It.IsAny<uint>())).Returns(Task.FromResult(new ImageResult()));

        var postService = new PostService(new Mock<ILogger<PostService>>().Object, mockPostClient.Object, mockImageClient.Object, new Mock<IUserService>().Object);

        // Act
        var unit = await postService.GetPost(123);

        // Assert
        Assert.IsNotNull(unit);
        Assert.IsNotNull(unit.Attachments);
        Assert.AreEqual(0, unit.Attachments.Count);
    }

    [TestMethod]
    public async Task PostServiceTests_GetPost_CanProcessAttachments()
    {
        // Arrange
        Mock<IPostDataClient> mockPostClient = new();
        mockPostClient.Setup(x => x.GetPost(It.IsAny<uint>())).Returns(Task.FromResult((BasePost)new ImagePost()));
        mockPostClient.Setup(x => x.GetPostAttachments(It.IsAny<uint>())).Returns(Task.FromResult(
            new List<PostAttachment>()
            {
                new PostAttachment() { AttachmentType = AttachmentType.Image },
                new PostAttachment() { AttachmentType = AttachmentType.Link },
            }
        ));
        mockPostClient.Setup(x => x.GetLinkByID(It.IsAny<uint>())).Returns(Task.FromResult(new Link()));

        Mock<IImageDataClient> mockImageClient = new();
        mockImageClient.Setup(x => x.GetImageByID(It.IsAny<uint>())).Returns(Task.FromResult(new ImageResult()));

        var postService = new PostService(new Mock<ILogger<PostService>>().Object, mockPostClient.Object, mockImageClient.Object, new Mock<IUserService>().Object);

        // Act
        var unit = await postService.GetPost(123);

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
