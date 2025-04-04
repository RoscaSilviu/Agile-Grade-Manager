using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CatalogueServer.Controllers;

namespace CatalogueServer.Tests.Controllers
{
    public class GradeDashboardControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IGradeRepository> _mockGradeRepository;
        private readonly GradeDashboardController _controller;

        public GradeDashboardControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockGradeRepository = new Mock<IGradeRepository>();
            _controller = new GradeDashboardController(_mockUserRepository.Object, _mockGradeRepository.Object);

            // Setup default HttpContext with empty headers
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public void GetGrades_ShouldReturnUnauthorized_WhenNoAuthorizationHeader()
        {
            // Act
            var result = _controller.GetGrades();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Missing or invalid token", unauthorizedResult.Value);
        }

        [Fact]
        public void GetGrades_ShouldReturnUnauthorized_WhenInvalidTokenFormat()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "InvalidFormat token123";

            // Act
            var result = _controller.GetGrades();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Missing or invalid token", unauthorizedResult.Value);
        }

        [Fact]
        public void GetGrades_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer validTokenFormat";

            _mockUserRepository
                .Setup(x => x.GetUserByToken("validTokenFormat"))
                .Returns((User)null);

            // Act
            var result = _controller.GetGrades();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid token", unauthorizedResult.Value);
        }

        [Fact]
        public void GetGrades_ShouldReturnGrades_WhenValidToken()
        {
            // Arrange
            var token = "validToken123";
            var userId = 1;
            var user = new User { Id = userId, Name = "Test User" };
            var grades = new List<(string Subject, double AverageGrade, DateTime LastGraded)>
            {
                ("Math", 85.5, DateTime.Now),
                ("Science", 90.0, DateTime.Now)
            };

            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockUserRepository
                .Setup(x => x.GetUserByToken(token))
                .Returns(user);

            _mockGradeRepository
                .Setup(x => x.GetStudentAverageGrades(userId))
                .Returns(grades);

            // Act
            var result = _controller.GetGrades();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var gradeResults = Assert.IsType<List<GradeAverageResult>>(okResult.Value);

            Assert.Equal(2, gradeResults.Count);
            Assert.Equal("Math", gradeResults[0].Subject);
            Assert.Equal(85.5, gradeResults[0].AverageGrade);
            Assert.Equal("Science", gradeResults[1].Subject);
            Assert.Equal(90.0, gradeResults[1].AverageGrade);
        }

        [Fact]
        public void GetGrades_ShouldReturnEmptyList_WhenNoGrades()
        {
            // Arrange
            var token = "validToken123";
            var userId = 1;
            var user = new User { Id = userId, Name = "Test User" };
            var grades = new List<(string Subject, double AverageGrade, DateTime LastGraded)>();

            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockUserRepository
                .Setup(x => x.GetUserByToken(token))
                .Returns(user);

            _mockGradeRepository
                .Setup(x => x.GetStudentAverageGrades(userId))
                .Returns(grades);

            // Act
            var result = _controller.GetGrades();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var gradeResults = Assert.IsType<List<GradeAverageResult>>(okResult.Value);
            Assert.Empty(gradeResults);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GetGrades_ShouldReturnUnauthorized_WhenTokenIsNullOrWhitespace(string token)
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            // Act
            var result = _controller.GetGrades();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid token", unauthorizedResult.Value);
        }
    }
}