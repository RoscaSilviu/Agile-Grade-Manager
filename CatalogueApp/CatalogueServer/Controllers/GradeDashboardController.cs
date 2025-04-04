using Microsoft.AspNetCore.Mvc;
using CatalogueServer.Repositories;

namespace CatalogueServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeDashboardController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IGradeRepository _gradeRepository;

        public GradeDashboardController(IUserRepository userRepository, IGradeRepository gradeRepository)
        {
            _userRepository = userRepository;
            _gradeRepository = gradeRepository;
        }

        [HttpGet]
        public IActionResult GetGrades()
        {
            // Extract the token from the Authorization header
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing or invalid token");
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            // Retrieve the user based on the token
            var user = _userRepository.GetUserByToken(token);
            if (user == null)
            {
                return Unauthorized("Invalid token");
            }

            // Get the grades for the user
            var grades = _gradeRepository.GetStudentAverageGrades(user.Id);

            // Map to the response model
            var result = grades.Select(g => new GradeAverageResult
            {
                Subject = g.Subject,
                AverageGrade = g.AverageGrade,
                LastGraded = g.LastGraded
            }).ToList();

            return Ok(result);
        }
    }

    public class GradeAverageResult
    {
        public string Subject { get; set; } = string.Empty;
        public double AverageGrade { get; set; }
        public DateTime LastGraded { get; set; }
    }
}