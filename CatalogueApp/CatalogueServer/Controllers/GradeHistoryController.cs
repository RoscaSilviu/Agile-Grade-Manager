using Microsoft.AspNetCore.Mvc;
using CatalogueServer.Repositories;

namespace CatalogueServer.Controllers
{
    /// <summary>
    /// Controller responsible for retrieving detailed grade history information for students.
    /// Provides endpoints for viewing grades by subject with detailed information.
    /// </summary>
    /// <remarks>
    /// This controller requires authentication via Bearer token and is designed
    /// to work with student accounts to display their detailed grade history.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class GradeHistoryController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IGradeRepository _gradeRepository;

        /// <summary>
        /// Initializes a new instance of the GradeHistoryController.
        /// </summary>
        /// <param name="userRepository">Repository for user operations.</param>
        /// <param name="gradeRepository">Repository for grade operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any repository is null.</exception>
        public GradeHistoryController(IUserRepository userRepository, IGradeRepository gradeRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
        }

        /// <summary>
        /// Retrieves the grade history for a specific subject for the authenticated student.
        /// </summary>
        /// <param name="subject">The name of the subject to get grades for.</param>
        /// <returns>
        /// 200 OK with detailed grade history;
        /// 401 Unauthorized if the authentication token is missing or invalid.
        /// </returns>
        /// <remarks>
        /// The endpoint requires a valid Bearer token in the Authorization header.
        /// Returns detailed grade information including assignment names, dates, and teacher comments.
        /// </remarks>
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

    /// <summary>
    /// Represents detailed grade history information for display.
    /// </summary>
    public class GradeHistoryResult
    {
        /// <summary>
        /// Gets or sets the name of the subject.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the numerical grade value.
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// Gets or sets the date when the grade was assigned.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the name of the assignment.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string AssignmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the teacher who gave the grade.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string TeacherName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets any additional comments about the grade.
        /// </summary>
        /// <remarks>
        /// Can be null if no comments were provided.
        /// </remarks>
        public string? Comments { get; set; }
    }
}