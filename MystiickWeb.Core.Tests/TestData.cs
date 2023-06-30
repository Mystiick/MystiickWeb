using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Core.Tests;

internal static class TestData
{
    internal static class User
    {
        public static readonly Guid ID = new("7ADA9356-80D0-4638-AE8A-08A4C7500C01");
        public const string Username = "Test Username";
        public const string PasswordHash = "Test Password Hash";
    }
}

internal static class PrebuiltMocks
{
    public static Mock<UserManager<User>> GetUserManager()
    {
        var userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User() { Username = TestData.User.Username, ID = TestData.User.ID }));
        userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));
        userManager.Setup(x => x.GetClaimsAsync(It.IsAny<User>())).Returns(Task.FromResult<IList<Claim>>(new List<Claim>()));

        return userManager;
    }

    public static SignInManager<User> GetSignInManager()
    {
        return new SignInManager<User>(
            GetUserManager().Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<User>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<User>>().Object
        );
    }
}
