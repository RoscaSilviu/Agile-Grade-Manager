using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CatalogueServer.Controllers;
using CatalogueServer.Repositories;

namespace CatalogueServer.Tests.Controllers
{
    public class ProfileControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly ProfileController _controller;

        public ProfileControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _controller = new ProfileController(_mockUserRepository.Object);

            // Setup default HttpContext with empty headers
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        #region GetProfile Tests

        [Fact]
        public void GetProfile_ShouldReturnUnauthorized_WhenNoAuthorizationHeader()
        {
            // Act
            var result = _controller.GetProfile();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Missing or invalid token", unauthorizedResult.Value);
        }

        [Fact]
        public void GetProfile_ShouldReturnUnauthorized_WhenInvalidTokenFormat()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "InvalidFormat token123";

            // Act
            var result = _controller.GetProfile();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Missing or invalid token", unauthorizedResult.Value);
        }

        [Fact]
        public void GetProfile_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer validTokenFormat";

            _mockUserRepository
                .Setup(x => x.GetUserByToken("validTokenFormat"))
                .Returns((User)null);

            // Act
            var result = _controller.GetProfile();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid token", unauthorizedResult.Value);
        }

        [Fact]
        public void GetProfile_ShouldReturnUserData_WhenValidToken()
        {
            // Arrange
            var token = "validToken123";
            var user = new User
            {
                Name = "John",
                Surname = "Doe",
                Email = "john@example.com",
                Role = "Student",
                LastLogin = DateTime.Now
            };

            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockUserRepository
                .Setup(x => x.GetUserByToken(token))
                .Returns(user);

            // Act
            var result = _controller.GetProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userResult = Assert.IsType<UserResult>(okResult.Value);

            Assert.Equal(user.Name, userResult.Name);
            Assert.Equal(user.Surname, userResult.Surname);
            Assert.Equal(user.Email, userResult.Email);
            Assert.Equal(user.Role, userResult.Role);
            Assert.Equal(user.LastLogin, userResult.LastLogin);
        }

        #endregion

        #region ChangePassword Tests

        [Fact]
        public void ChangePassword_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.ChangePassword(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request", badRequestResult.Value);
        }

        [Theory]
        [InlineData("", "newPassword")]
        [InlineData("currentPassword", "")]
        [InlineData(null, "newPassword")]
        [InlineData("currentPassword", null)]
        [InlineData(" ", "newPassword")]
        [InlineData("currentPassword", " ")]
        public void ChangePassword_ShouldReturnBadRequest_WhenPasswordsAreInvalid(string currentPassword, string newPassword)
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            // Act
            var result = _controller.ChangePassword(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request", badRequestResult.Value);
        }

        [Fact]
        public void ChangePassword_ShouldReturnUnauthorized_WhenNoToken()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "current",
                NewPassword = "new"
            };

            // Act
            var result = _controller.ChangePassword(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Missing or invalid token", unauthorizedResult.Value);
        }

        [Fact]
        public void ChangePassword_ShouldReturnBadRequest_WhenCurrentPasswordIsIncorrect()
        {
            // Arrange
            var token = "validToken123";
            var user = new User
            {
                Password = PasswordHelper.HashPassword("correctPassword")
            };

            var request = new ChangePasswordRequest
            {
                CurrentPassword = "wrongPassword",
                NewPassword = "newPassword"
            };

            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockUserRepository
                .Setup(x => x.GetUserByToken(token))
                .Returns(user);

            // Act
            var result = _controller.ChangePassword(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current password is incorrect", badRequestResult.Value);
        }

        [Fact]
        public void ChangePassword_ShouldUpdatePassword_WhenAllIsValid()
        {
            // Arrange
            var token = "validToken123";
            var currentPassword = "currentPassword";
            var newPassword = "newPassword";

            var user = new User
            {
                Password = PasswordHelper.HashPassword(currentPassword)
            };

            var request = new ChangePasswordRequest
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockUserRepository
                .Setup(x => x.GetUserByToken(token))
                .Returns(user);

            User updatedUser = null;
            _mockUserRepository
                .Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u);

            // Act
            var result = _controller.ChangePassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password changed successfully", okResult.Value);

            // Verify password was updated and hashed
            Assert.NotNull(updatedUser);
            Assert.True(PasswordHelper.ValidatePassword(newPassword, updatedUser.Password));

            // Verify Update was called
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        #endregion
    }
}