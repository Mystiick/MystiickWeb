using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using MystiickWeb.Core.Services;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Tests.Services;

[TestClass]
public class UserServiceTests
{

    #region | AuthenticateUser |
    [TestMethod]
    public async Task UserService_AuthenticateUser_Pass()
    {
        // Arrange
        var userManager = PrebuiltMocks.GetUserManager();
        IOptions<Features> features = Options.Create(new Features());

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, features, null);
        ClaimsIdentity output;

        // Act
        output = await unit.AuthenticateUser(new Credential());

        // Assert
        Assert.AreEqual(TestData.User.ID.ToString(), output.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
        Assert.AreEqual(TestData.User.Username, output.Name);
        userManager.Verify(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task UserService_AuthenticateUser_Unauthenticated_ThrowsException()
    {
        // Arrange
        var userManager = PrebuiltMocks.GetUserManager();
        userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

        IOptions<Features> features = Options.Create(new Features());

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, features, new Mock<SignInManager<User>>().Object);
        UnauthorizedAccessException? expectedException = null;

        // Act
        try
        {
            await unit.AuthenticateUser(new Credential());
        }
        catch (UnauthorizedAccessException ex)
        {
            expectedException = ex;
        }

        // Assert
        Assert.IsNotNull(expectedException);
        userManager.Verify(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task UserService_AuthenticateUser_LockedAccount_ThrowsException()
    {
        // Arrange
        var userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User() { LockoutEndDate = DateTimeOffset.UtcNow.AddDays(1) }));

        IOptions<Features> features = Options.Create(new Features());

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, features, new Mock<SignInManager<User>>().Object);
        UnauthorizedAccessException? expectedException = null;

        // Act
        try
        {
            await unit.AuthenticateUser(new Credential());
        }
        catch (UnauthorizedAccessException ex)
        {
            expectedException = ex;
        }

        // Assert
        Assert.IsNotNull(expectedException);
        userManager.Verify(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }
    #endregion

    #region | RegisterUser |
    [TestMethod]
    public async Task UserService_RegisterUser_Pass()
    {
        // Arrange
        var userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));
        userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));

        IOptions<Features> features = Options.Create(new Features() { UserRegistration = true });

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, features, new Mock<SignInManager<User>>().Object);
        List<string> output;

        // Act
        output = await unit.RegisterUser(new Credential());

        // Assert
        Assert.AreEqual(0, output.Count);
    }

    [TestMethod]
    public async Task UserService_RegisterUser_AlreadyExists()
    {
        // Arrange
        var userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User() { Username = TestData.User.Username, ID = TestData.User.ID }));

        IOptions<Features> features = Options.Create(new Features() { UserRegistration = true });

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, features, new Mock<SignInManager<User>>().Object);
        List<string> output;

        // Act
        output = await unit.RegisterUser(new Credential());

        // Assert
        Assert.AreEqual(1, output.Count);
    }
    #endregion

    #region | UpdateUsername |
    [TestMethod]
    public async Task UserService_UpdateUsername_Pass()
    {
        // Arrange
        var userManager = PrebuiltMocks.GetUserManager();
        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, Options.Create(new Features()), new Mock<SignInManager<User>>().Object);

        // Act
        await unit.UpdateUsername(new Credential(), "TEST");

        // Assert
        userManager.Verify(x => x.FindByNameAsync(It.IsAny<string>()));
        userManager.Verify(x => x.UpdateAsync(It.IsAny<User>()));
    }

    [TestMethod]
    public async Task UserService_UpdateUsername_InvalidCredentials_ThrowsException()
    {
        // Arrange
        var userManager = PrebuiltMocks.GetUserManager();
        userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, Options.Create(new Features()), new Mock<SignInManager<User>>().Object);
        UnauthorizedAccessException? expectedException = null;

        // Act
        try
        {
            await unit.UpdateUsername(new Credential(), "TEST");
        }
        catch (UnauthorizedAccessException ex)
        {
            expectedException = ex;
        }

        // Assert
        Assert.IsNotNull(expectedException);
        userManager.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public async Task UserService_UpdateUsername_LockedAccount_ThrowsException()
    {
        // Arrange
        var userManager = PrebuiltMocks.GetUserManager();
        userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User() { LockoutEndDate = DateTimeOffset.UtcNow.AddDays(1) }));

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, Options.Create(new Features()), new Mock<SignInManager<User>>().Object);
        UnauthorizedAccessException? expectedException = null;

        // Act
        try
        {
            await unit.UpdateUsername(new Credential(), "TEST");
        }
        catch (UnauthorizedAccessException ex)
        {
            expectedException = ex;
        }

        // Assert
        Assert.IsNotNull(expectedException);
        userManager.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
    #endregion

    #region | UpdatePassword |
    [TestMethod]
    public async Task UserService_UpdatePassword_Pass()
    {
        // Arrange
        var userManager = PrebuiltMocks.GetUserManager();

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, Options.Create(new Features()), new Mock<SignInManager<User>>().Object);

        // Act
        await unit.UpdatePassword(new Credential(), new Credential() { Password = "TEST", ConfirmPassword = "TEST" });

        // Assert
        userManager.Verify(x => x.FindByNameAsync(It.IsAny<string>()));
        userManager.Verify(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), "TEST"));
    }

    [TestMethod]
    public async Task UserService_UpdatePassword_InvalidCredentials_ThrowsException()
    {
        // Arrange
        var userManager = PrebuiltMocks.GetUserManager();
        userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, Options.Create(new Features()), new Mock<SignInManager<User>>().Object);
        UnauthorizedAccessException? expectedException = null;

        // Act
        try
        {
            await unit.UpdatePassword(new Credential(), new Credential() { Password = "TEST", ConfirmPassword = "TEST" });
        }
        catch (UnauthorizedAccessException ex)
        {
            expectedException = ex;
        }

        // Assert
        Assert.IsNotNull(expectedException);
        userManager.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        userManager.Verify(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task UserService_UpdatePassword_PasswordDoesntMatch_ThrowsException()
    {
        // Arrange
        var userManager = PrebuiltMocks.GetUserManager();

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, Options.Create(new Features()), new Mock<SignInManager<User>>().Object);
        UnauthorizedAccessException? expectedException = null;

        // Act
        try
        {
            await unit.UpdatePassword(new Credential(), new Credential() { Password = "TEST", ConfirmPassword = "Mismatch Password" });
        }
        catch (UnauthorizedAccessException ex)
        {
            expectedException = ex;
        }

        // Assert
        Assert.IsNotNull(expectedException);
        userManager.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        userManager.Verify(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task UserService_UpdatePassword_LockedAccount_ThrowsException()
    {
        // Arrange
        var userManager = PrebuiltMocks.GetUserManager();
        userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User() { LockoutEndDate = DateTimeOffset.UtcNow.AddDays(1) }));

        var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object, Options.Create(new Features()), new Mock<SignInManager<User>>().Object);
        UnauthorizedAccessException? expectedException = null;

        // Act
        try
        {
            await unit.UpdatePassword(new Credential(), new Credential() { Password = "TEST", ConfirmPassword = "TEST" });
        }
        catch (UnauthorizedAccessException ex)
        {
            expectedException = ex;
        }

        // Assert
        Assert.IsNotNull(expectedException);
        userManager.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
    #endregion
}
