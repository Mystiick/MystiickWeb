﻿using Microsoft.AspNetCore.Identity;


using MystiickWeb.Core.Services;
using MystiickWeb.Shared.Models.User;

using System.Security.Claims;

namespace MystiickWeb.Core.Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public async Task UserService_AuthenticateUser_Pass()
        {
            // Arrange
            var userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User() { Username = TestData.User.Username, ID = TestData.User.ID }));
            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object);
            ClaimsIdentity output;

            // Act
            output = await unit.AuthenticateUser(new Credential());

            // Assert
            Assert.AreEqual(TestData.User.ID.ToString(), output.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            Assert.AreEqual(TestData.User.Username, output.Name);
        }

        [TestMethod]
        public async Task UserService_AuthenticateUser_Unauthenticated_ThrowsException()
        {
            // Arrange
            var userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User() { Username = TestData.User.Username, ID = TestData.User.ID }));
            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object);
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
        }


        [TestMethod]
        public async Task UserService_RegisterUser_Pass()
        {
            // Arrange
            var userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));

            var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object);
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

            var unit = new UserService(new Mock<ILogger<UserService>>().Object, userManager.Object);
            List<string> output;

            // Act
            output = await unit.RegisterUser(new Credential());

            // Assert
            Assert.AreEqual(1, output.Count);
        }
    }
}
