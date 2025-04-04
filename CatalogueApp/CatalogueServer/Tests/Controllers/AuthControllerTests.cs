using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using CatalogueServer.Controllers;

namespace CatalogueServer.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _controller = new AuthController(_mockUserRepository.Object);
        }

        [Fact]
        public void Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var hashedPassword = PasswordHelper.HashPassword("correctPassword");
            var user = new User
            {
                Email = "test@example.com",
                Password = hashedPassword,
                Role = "Teacher"
            };

            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "correctPassword"
            };

            _mockUserRepository
                .Setup(x => x.GetUserByEmail(loginRequest.Email))
                .Returns(user);

            _mockUserRepository
                .Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(u =>
                {
                    // Verify user properties are updated
                    Assert.NotNull(u.Token);
                    Assert.True(u.LastLogin <= DateTime.Now);
                });

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.NotNull(value.Token);
            Assert.Equal("Teacher", value.Role);

            // Verify Update was called
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void Login_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "password"
            };

            _mockUserRepository
                .Setup(x => x.GetUserByEmail(loginRequest.Email))
                .Returns((User)null);

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid email or password", unauthorizedResult.Value);

            // Verify Update was not called
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            var hashedPassword = PasswordHelper.HashPassword("correctPassword");
            var user = new User
            {
                Email = "test@example.com",
                Password = hashedPassword,
                Role = "Teacher"
            };

            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "wrongPassword"
            };

            _mockUserRepository
                .Setup(x => x.GetUserByEmail(loginRequest.Email))
                .Returns(user);

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid email or password", unauthorizedResult.Value);

            // Verify Update was not called
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void Login_ShouldUpdateUserTokenAndLastLogin_WhenSuccessful()
        {
            // Arrange
            var hashedPassword = PasswordHelper.HashPassword("correctPassword");
            var initialToken = "old-token";
            var initialLastLogin = DateTime.Now.AddDays(-1);

            var user = new User
            {
                Email = "test@example.com",
                Password = hashedPassword,
                Role = "Teacher",
                Token = initialToken,
                LastLogin = initialLastLogin
            };

            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "correctPassword"
            };

            User updatedUser = null;
            _mockUserRepository
                .Setup(x => x.GetUserByEmail(loginRequest.Email))
                .Returns(user);

            _mockUserRepository
                .Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u);

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.NotEqual(initialToken, updatedUser.Token);
            Assert.True(updatedUser.LastLogin > initialLastLogin);
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData("test@example.com", "")]
        [InlineData(null, "password")]
        [InlineData("test@example.com", null)]
        public void Login_ShouldHandleInvalidInput(string email, string password)
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}