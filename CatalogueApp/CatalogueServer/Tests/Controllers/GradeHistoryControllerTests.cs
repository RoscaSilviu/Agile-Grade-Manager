using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CatalogueServer.Controllers;
using CatalogueServer.Repositories;

namespace CatalogueServer.Tests.Controllers
{
    public class GradeHistoryControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IGradeRepository> _mockGradeRepository;
        private readonly GradeHistoryController _controller;

        public GradeHistoryControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockGradeRepository = new Mock<IGradeRepository>();
            _controller = new GradeHistoryController(_mockUserRepository.Object, _mockGradeRepository.Object);

            // Setup default HttpContext with empty headers
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public void GetGradeHistory_ShouldReturnUnauthorized_WhenNoAuthorizationHeader()
        {
            // Act
            var result = _controller.GetGradeHistory("Math");

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Missing or invalid token", unauthorizedResult.Value);
        }

        [Fact]
        public void GetGradeHistory_ShouldReturnUnauthorized_WhenInvalidTokenFormat()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "InvalidFormat token123";

            // Act
            var result = _controller.GetGradeHistory("Math");

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Missing or invalid token", unauthorizedResult.Value);
        }

        [Fact]
        public void GetGradeHistory_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer validTokenFormat";

            _mockUserRepository
                .Setup(x => x.GetUserByToken("validTokenFormat"))
                .Returns((User)null);

            // Act
            var result = _controller.GetGradeHistory("Math");

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid token", unauthorizedResult.Value);
        }

        [Fact]
        public void GetGradeHistory_ShouldReturnGradeHistory_WhenValidToken()
        {
            // Arrange
            var token = "validToken123";
            var userId = 1;
            var subject = "Math";
            var user = new User { Id = userId, Name = "Test User" };
            var gradeDetails = new List<GradeRepository.GradeDetail>
            {
                new GradeRepository.GradeDetail
                {
                    Subject = subject,
                    Value = 85,
                    Date = DateTime.Now.AddDays(-1),
                    AssignmentName = "Test 1",
                    TeacherName = "Mr. Smith",
                    Comments = "Good work"
                },
                new GradeRepository.GradeDetail
                {
                    Subject = subject,
                    Value = 90,
                    Date = DateTime.Now,
                    AssignmentName = "Test 2",
                    TeacherName = "Mr. Smith",
                    Comments = "Excellent"
                }
            };

            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockUserRepository
                .Setup(x => x.GetUserByToken(token))
                .Returns(user);

            _mockGradeRepository
                .Setup(x => x.GetStudentGradesBySubject(userId, subject))
                .Returns(gradeDetails);

            // Act
            var result = _controller.GetGradeHistory(subject);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var historyResults = Assert.IsType<List<GradeHistoryResult>>(okResult.Value);

            Assert.Equal(2, historyResults.Count);

            // Verify first grade
            Assert.Equal(subject, historyResults[0].Subject);
            Assert.Equal(85, historyResults[0].Grade);
            Assert.Equal("Test 1", historyResults[0].AssignmentName);
            Assert.Equal("Mr. Smith", historyResults[0].TeacherName);
            Assert.Equal("Good work", historyResults[0].Comments);

            // Verify second grade
            Assert.Equal(90, historyResults[1].Grade);
            Assert.Equal("Test 2", historyResults[1].AssignmentName);
            Assert.Equal("Excellent", historyResults[1].Comments);
        }

        [Fact]
        public void GetGradeHistory_ShouldReturnEmptyList_WhenNoGrades()
        {
            // Arrange
            var token = "validToken123";
            var userId = 1;
            var subject = "Math";
            var user = new User { Id = userId, Name = "Test User" };
            var gradeDetails = new List<GradeRepository.GradeDetail>();

            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockUserRepository
                .Setup(x => x.GetUserByToken(token))
                .Returns(user);

            _mockGradeRepository
                .Setup(x => x.GetStudentGradesBySubject(userId, subject))
                .Returns(gradeDetails);

            // Act
            var result = _controller.GetGradeHistory(subject);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var historyResults = Assert.IsType<List<GradeHistoryResult>>(okResult.Value);
            Assert.Empty(historyResults);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GetGradeHistory_ShouldHandleInvalidSubject(string subject)
        {
            // Arrange
            var token = "validToken123";
            var userId = 1;
            var user = new User { Id = userId, Name = "Test User" };

            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockUserRepository
                .Setup(x => x.GetUserByToken(token))
                .Returns(user);

            _mockGradeRepository
                .Setup(x => x.GetStudentGradesBySubject(userId, subject))
                .Returns(new List<GradeRepository.GradeDetail>());

            // Act
            var result = _controller.GetGradeHistory(subject);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var historyResults = Assert.IsType<List<GradeHistoryResult>>(okResult.Value);
            Assert.Empty(historyResults);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GetGradeHistory_ShouldReturnUnauthorized_WhenTokenIsNullOrWhitespace(string token)
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            // Act
            var result = _controller.GetGradeHistory("Math");

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid token", unauthorizedResult.Value);
        }
    }
}