using Microsoft.AspNetCore.Mvc;
using CatalogueServer.Repositories;
using static CatalogueServer.Repositories.GradeRepository;


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

        // ========== TEACHER-SPECIFIC ==========

        [HttpGet("teacher/{teacherId}")]
        public ActionResult<List<TeacherGradeDetail>> GetTeacherGrades(int teacherId)
        {
            return _gradeRepository.GetTeacherGradeDetails(teacherId);
        }

        [HttpPut("teacher/update/{gradeId}")]
        public IActionResult UpdateGrade(int gradeId, [FromBody] int newValue)
        {
            _gradeRepository.UpdateGrade(gradeId, newValue);
            return Ok();
        }

        [HttpDelete("teacher/{gradeId}")]
        public IActionResult DeleteGrade(int gradeId)
        {
            _gradeRepository.DeleteGrade(gradeId);
            return Ok();
        }

        [HttpPost("teacher/bulk-upload")]
        public async Task<IActionResult> BulkUploadGrades([FromBody] List<GradeUploadModel> grades)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized("Missing or invalid token");
                }

                string token = authHeader.Substring("Bearer ".Length).Trim();
                var teacher = _userRepository.GetUserByToken(token);
                if (teacher == null || teacher.Role != "teacher")
                {
                    return Unauthorized("Invalid token or not a teacher");
                }

                var gradeEntities = new List<Grade>();
                foreach (var grade in grades)
                {
                    // Look up student by name using the repository
                    var student = _userRepository.GetStudentsByTeacherId(teacher.Id)
                        .FirstOrDefault(s => $"{s.Name} {s.Surname}" == grade.StudentName);
                    if (student == null) continue;

                    // Look up assignment using the repository method (to be added to IGradeRepository)
                    var assignment = _gradeRepository.GetAssignmentByNameAndTeacher(grade.AssignmentName, teacher.Id);
                    if (assignment == null) continue;

                    gradeEntities.Add(new Grade
                    {
                        StudentId = student.Id,
                        AssignmentId = assignment.Id,
                        Value = grade.Grade,
                        Date = DateTime.Now
                    });
                }

                // Use the repository's bulk insert method
                _gradeRepository.BulkGradeAssignments(gradeEntities);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to process grades");
            }
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

    /// <summary>
    /// Represents detailed grade information for a teacher's class.
    /// </summary>
    /// <remarks>
    /// This class is used to provide detailed information about grades
    /// assigned by a teacher, including the student, assignment, and grade details.
    /// </remarks>
    public class TeacherGradeDetail
    {
        /// <summary>
        /// Gets or sets the unique identifier for the grade.
        /// </summary>
        /// <remarks>
        /// This is used to identify the grade entry for editing or deletion.
        /// </remarks>
        public int GradeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the student who received the grade.
        /// </summary>
        /// <remarks>
        /// This includes the student's first and last name for display purposes.
        /// </remarks>
        public string StudentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the subject associated with the grade.
        /// </summary>
        /// <remarks>
        /// This is used to group grades by subject in the teacher's class.
        /// </remarks>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the assignment for which the grade was given.
        /// </summary>
        /// <remarks>
        /// This is used to identify the specific assignment in the teacher's class.
        /// </remarks>
        public string AssignmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the numerical value of the grade.
        /// </summary>
        /// <remarks>
        /// Represents the score or grade assigned to the student for the assignment.
        /// </remarks>
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets the date when the grade was assigned.
        /// </summary>
        /// <remarks>
        /// This is used to track when the grade was recorded.
        /// </remarks>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets any comments provided by the teacher regarding the grade.
        /// </summary>
        /// <remarks>
        /// Comments can include feedback or notes about the student's performance.
        /// </remarks>
        public string? Comments { get; set; }
    }

    /// <summary>
    /// Represents a grade entry for bulk upload operations.
    /// </summary>
    public class GradeUploadModel
    {
        /// <summary>
        /// Gets or sets the full name of the student.
        /// </summary>
        /// <remarks>
        /// Must match the format "FirstName Surname" as stored in the database.
        /// </remarks>
        public string StudentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the assignment.
        /// </summary>
        /// <remarks>
        /// Must match an existing assignment name in the teacher's classes.
        /// </remarks>
        public string AssignmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the numerical grade value.
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// Gets or sets any comments about the grade.
        /// </summary>
        public string? Comments { get; set; }
    }
}