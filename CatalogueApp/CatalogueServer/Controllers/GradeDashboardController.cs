using Microsoft.AspNetCore.Mvc;
using CatalogueServer.Repositories;

namespace CatalogueServer.Controllers
{
    /// <summary>
    /// Controller responsible for managing the grade dashboard functionality.
    /// Provides endpoints for retrieving student grade averages and summaries.
    /// </summary>
    /// <remarks>
    /// This controller requires authentication via Bearer token and is designed
    /// to work with student accounts to display their grade information.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class GradeDashboardController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IGradeRepository _gradeRepository;

        /// <summary>
        /// Initializes a new instance of the GradeDashboardController.
        /// </summary>
        /// <param name="userRepository">Repository for user operations.</param>
        /// <param name="gradeRepository">Repository for grade operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any repository is null.</exception>
        public GradeDashboardController(IUserRepository userRepository, IGradeRepository gradeRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
        }

        /// <summary>
        /// Retrieves the average grades for all subjects for the authenticated student.
        /// </summary>
        /// <returns>
        /// 200 OK with a list of grade averages per subject;
        /// 401 Unauthorized if the authentication token is missing or invalid.
        /// </returns>
        /// <remarks>
        /// The endpoint requires a valid Bearer token in the Authorization header.
        /// Returns average grades, including subjects with no grades as 0.0.
        /// </remarks>
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

    /// <summary>
    /// Represents the average grade information for a subject.
    /// </summary>
    public class GradeAverageResult
    {
        /// <summary>
        /// Gets or sets the name of the subject.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the calculated average grade for the subject.
        /// </summary>
        /// <remarks>
        /// Represents the weighted average of all assignments in the subject.
        /// </remarks>
        public double AverageGrade { get; set; }

        /// <summary>
        /// Gets or sets the date of the most recent grade in this subject.
        /// </summary>
        public DateTime LastGraded { get; set; }
    }
}