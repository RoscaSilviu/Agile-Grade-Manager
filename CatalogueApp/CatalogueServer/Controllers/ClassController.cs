using Microsoft.AspNetCore.Mvc;
using CatalogueServer.Repositories;

namespace CatalogueServer.Controllers
{
    /// <summary>
    /// Controller responsible for managing class-related operations in the school management system.
    /// Provides endpoints for class management, student enrollment, and assignment handling.
    /// </summary>
    /// <remarks>
    /// Most endpoints require authentication via Bearer token and are designed for teacher access.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly ClassRepository _classRepository;
        private readonly UserRepository _userRepository;
        private readonly AssignmentRepository _assignmentRepository;

        private string _token;

        /// <summary>
        /// Initializes a new instance of the ClassController.
        /// </summary>
        /// <param name="classRepository">Repository for class operations.</param>
        /// <param name="userRepository">Repository for user operations.</param>
        /// <param name="assignmentRepository">Repository for assignment operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any repository is null.</exception>
        public ClassController(ClassRepository classRepository, UserRepository userRepository, AssignmentRepository assignmentRepository)
        {
            _classRepository = classRepository ?? throw new ArgumentNullException(nameof(classRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        }

        /// <summary>
        /// Extracts the Bearer token from the Authorization header.
        /// </summary>
        /// <returns>The token if present and valid; null otherwise.</returns>
        private string GetTokenFromHeader()
        {
            var authHeader = Request.Headers["Authorization"];
            return authHeader.Count > 0 && authHeader[0].StartsWith("Bearer ")
                ? authHeader[0].Substring("Bearer ".Length).Trim()
                : null;
        }

        /// <summary>
        /// Retrieves a specific class by its name for the authenticated teacher.
        /// </summary>
        /// <param name="name">The name of the class to retrieve.</param>
        /// <returns>200 OK with class details; 401 Unauthorized if token invalid; 404 NotFound if class doesn't exist.</returns>
        [HttpGet("{name}")]
        public ActionResult GetClassByName(string name)
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing or invalid token");
            }
            string token = GetTokenFromHeader();
            var user = _userRepository.GetUserByToken(token);
            if (user == null)
            {
                return Unauthorized("Invalid token");
            }
            var classObj = _classRepository.GetClassByNameAndTeacherId(name, user.Id);
            if (classObj == null)
            {
                return NotFound("Class not found");
            }
            return Ok(classObj);
        }

        /// <summary>
        /// Retrieves all classes for the authenticated teacher.
        /// </summary>
        /// <returns>200 OK with list of classes; 401 Unauthorized if token invalid.</returns>
        [HttpGet]
        public IActionResult GetClasses()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing or invalid token");
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();
            _token = token;
            var user = _userRepository.GetUserByToken(token);

            if (user == null)
            {
                return Unauthorized("Invalid token");
            }

            var classes = _classRepository.GetClassesByTeacherId(user.Id);
            return Ok(classes);
        }

        /// <summary>
        /// Creates a new class for the authenticated teacher.
        /// </summary>
        /// <param name="name">The name of the new class.</param>
        /// <returns>200 OK if created successfully; 401 Unauthorized if token invalid.</returns>
        [HttpPost("AddClass")]
        public IActionResult PostClass([FromBody] string name)
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing or invalid token");
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();
            Class newClass = new Class
            {
                Name = name,
                TeacherId = _userRepository.GetUserByToken(token).Id
            };
            _classRepository.Insert(newClass);
            return Ok();
        }

        /// <summary>
        /// Retrieves all students in the system.
        /// </summary>
        /// <returns>200 OK with list of students.</returns>
        [HttpGet("GetStudents")]
        public IActionResult GetStudents()
        {
            var students = _userRepository.GetAllStudents();
            return Ok(students);
        }

        /// <summary>
        /// Deletes a class by its name.
        /// </summary>
        /// <param name="name">The name of the class to delete.</param>
        /// <returns>200 OK if deleted; 401 Unauthorized if token invalid; 404 NotFound if class not found.</returns>
        [HttpDelete("DeleteClass")]
        public IActionResult DeleteClass([FromQuery] string name)
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing or invalid token");
            }
            string token = authHeader.Substring("Bearer ".Length).Trim();
            Class classToDelete = _classRepository.GetClassByNameAndTeacherId(name, _userRepository.GetUserByToken(token).Id);
            if (classToDelete == null)
            {
                return NotFound("Class not found");
            }
            _classRepository.Delete(classToDelete);
            return Ok();
        }

        /// <summary>
        /// Adds a student to a specific class.
        /// </summary>
        /// <param name="request">The enrollment request containing class and student details.</param>
        /// <returns>200 OK if added; 404 NotFound if class or student not found.</returns>
        [HttpPost("AddStudentToClass")]
        public IActionResult AddStudentToClass([FromBody] StudentClassRequest request)
        {
            var student = _userRepository.GetStudentByName(request.FirstName, request.LastName);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            string token = GetTokenFromHeader();
            var teacher = _userRepository.GetUserByToken(token);

            Class classToAddStudent = _classRepository.GetClassByNameAndTeacherId(
                request.ClassName,
                teacher.Id
            );

            if (classToAddStudent == null)
            {
                return NotFound("Class not found");
            }

            _classRepository.AddStudentToClass(classToAddStudent.Id, student.Id);
            return Ok();
        }

        /// <summary>
        /// Retrieves all students enrolled in a specific class.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <returns>200 OK with list of students; 404 NotFound if no students found.</returns>
        [HttpGet("GetStudentsInClass/{className}")]
        public IActionResult GetStudentsInClass(string className)
        {
            int teacherId = _userRepository.GetUserByToken(GetTokenFromHeader()).Id;
            int classId = _classRepository.GetClassByNameAndTeacherId(className, teacherId)?.Id ?? -1;

            var students = _classRepository.GetStudentsInClass(classId);

            if (students == null || students.Count == 0)
            {
                return NotFound("No students found in this class");
            }
            else
            {
                return Ok(students);
            }
        }

        /// <summary>
        /// Retrieves all students not enrolled in a specific class.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <returns>200 OK with list of students; 404 NotFound if no students found.</returns>
        [HttpGet("GetAllStudentsNotInClass/{className}")]
        public IActionResult GetAllStudentsNotInClass(string className)
        {
            int teacherId = _userRepository.GetUserByToken(GetTokenFromHeader()).Id;

            int classId = _classRepository.GetClassByNameAndTeacherId(className, teacherId)?.Id ?? -1;
            var students = _userRepository.GetAllStudents();
            var studentsInClass = _classRepository.GetStudentsInClass(classId);
            var studentsNotInClass = students.Where(s => !studentsInClass.Any(sc => sc.Id == s.Id)).ToList();
            if (studentsNotInClass == null || studentsNotInClass.Count == 0)
            {
                return NotFound("No students found not in this class");
            }
            else
            {
                return Ok(studentsNotInClass);
            }
        }

        /// <summary>
        /// Retrieves all students in the system.
        /// </summary>
        /// <returns>200 OK with list of students; 404 NotFound if no students found.</returns>
        [HttpGet("GetAllStudents")]
        public IActionResult GetAllStudents()
        {
            var students = _userRepository.GetAllStudents();
            if (students == null || students.Count == 0)
            {
                return NotFound("No students found");
            }
            else
            {
                return Ok(students);
            }
        }

        /// <summary>
        /// Removes a student from a class.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <param name="firstName">The student's first name.</param>
        /// <param name="lastName">The student's last name.</param>
        /// <returns>200 OK if removed; various error status codes for different failure cases.</returns>
        [HttpDelete("RemoveStudentFromClass")]
        public IActionResult RemoveStudentFromClass(
            [FromQuery] string className,
            [FromQuery] string firstName,
            [FromQuery] string lastName)
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
                return Unauthorized();

            string token = GetTokenFromHeader();
            var teacher = _userRepository.GetUserByToken(token);
            if (teacher == null) return Unauthorized("Invalid token");

            var classObj = _classRepository.GetClassByNameAndTeacherId(className, teacher.Id);
            if (classObj == null) return NotFound("Class not found");

            var student = _userRepository.GetStudentByName(firstName, lastName);
            if (student == null) return NotFound("Student not found");

            try
            {
                _classRepository.RemoveStudentFromClass(classObj.Id, student.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error removing student: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all assignments for a specific class.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <returns>200 OK with list of assignments.</returns>
        [HttpGet("{className}/assignments")]
        public IActionResult GetClassAssignments(string className)
        {
            string token = GetTokenFromHeader();
            var teacher = _userRepository.GetUserByToken(token);
            var classObj = _classRepository.GetClassByNameAndTeacherId(className, teacher.Id);

            var assignments = _assignmentRepository.GetAssignmentsByClassId(classObj.Id);
            return Ok(assignments);
        }

        /// <summary>
        /// Adds a new assignment to a class.
        /// </summary>
        /// <param name="assignment">The assignment details.</param>
        /// <returns>200 OK if created; 404 NotFound if class not found.</returns>
        [HttpPost("AddAssignment")]
        public IActionResult AddAssignment([FromBody] AssignmentResponse assignment)
        {
            string token = GetTokenFromHeader();
            var teacher = _userRepository.GetUserByToken(token);

            var classObj = _classRepository.GetClassByNameAndTeacherId(assignment.ClassName, teacher.Id);
            if (classObj == null)
            {
                return NotFound("Class not found");
            }

            Assignment newAssignment = new Assignment
            {
                Name = assignment.Title,
                Description = assignment.Description,
                DueDate = assignment.DueDate,
                ClassId = classObj.Id
            };

            _assignmentRepository.Insert(newAssignment);
            return Ok();
        }

        /// <summary>
        /// Updates an existing assignment.
        /// </summary>
        /// <param name="id">The ID of the assignment to update.</param>
        /// <param name="assignment">The updated assignment details.</param>
        /// <returns>204 NoContent if updated; 400 BadRequest if ID mismatch.</returns>
        [HttpPut("UpdateAssignment/{id}")]
        public IActionResult UpdateAssignment(int id, [FromBody] Assignment assignment)
        {
            if (id != assignment.Id) return BadRequest();

            _assignmentRepository.Update(assignment);
            return NoContent();
        }

        /// <summary>
        /// Deletes an assignment.
        /// </summary>
        /// <param name="id">The ID of the assignment to delete.</param>
        /// <returns>204 NoContent if deleted; 404 NotFound if not found.</returns>
        [HttpDelete("DeleteAssignment/{id}")]
        public IActionResult DeleteAssignment(int id)
        {
            var assignment = _assignmentRepository.GetById(id);
            if (assignment == null) return NotFound();

            _assignmentRepository.Delete(assignment);
            return NoContent();
        }
    }

    /// <summary>
    /// Represents a request to enroll a student in a class.
    /// </summary>
    public class StudentClassRequest
    {
        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the student's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the student's last name.
        /// </summary>
        public string LastName { get; set; }
    }

    /// <summary>
    /// Represents an assignment creation or update request.
    /// </summary>
    public class AssignmentResponse
    {
        /// <summary>
        /// Gets or sets the assignment ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the assignment.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the assignment.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the due date of the assignment.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the class this assignment belongs to.
        /// </summary>
        public string ClassName { get; set; }
    }
}