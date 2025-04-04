using Microsoft.AspNetCore.Mvc;
using CatalogueServer.Repositories;

namespace CatalogueServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeHistoryController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly GradeRepository _gradeRepository;

        public GradeHistoryController(UserRepository userRepository, GradeRepository gradeRepository)
        {
            _userRepository = userRepository;
            _gradeRepository = gradeRepository;
        }

        [HttpGet("{subject}")]
        public IActionResult GetGradeHistory(string subject)
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing or invalid token");
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();
            var user = _userRepository.GetUserByToken(token);
            if (user == null)
            {
                return Unauthorized("Invalid token");
            }

            var grades = _gradeRepository.GetStudentGradesBySubject(user.Id, subject);

            // The mapping is simpler now since GradeDetail matches GradeHistoryResult
            var result = grades.Select(g => new GradeHistoryResult
            {
                Subject = g.Subject,
                Grade = g.Value,
                Date = g.Date,
                AssignmentName = g.AssignmentName,
                TeacherName = g.TeacherName,
                Comments = g.Comments
            }).ToList();

            return Ok(result);
        }
    }

    public class GradeHistoryResult
    {
        public string Subject { get; set; } = string.Empty;
        public int Grade { get; set; }
        public DateTime Date { get; set; }
        public string AssignmentName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }
}