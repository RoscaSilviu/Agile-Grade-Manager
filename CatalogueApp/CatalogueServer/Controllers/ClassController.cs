using Microsoft.AspNetCore.Mvc;
using CatalogueServer.Repositories;

namespace CatalogueServer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly ClassRepository _classRepository;
        private readonly UserRepository _userRepository;
        private readonly AssignmentRepository _assignmentRepository;

        private string _token;

        public ClassController(ClassRepository classRepository, UserRepository userRepository, AssignmentRepository assignmentRepository)
        {
            _classRepository = classRepository;
            _userRepository = userRepository;
            _assignmentRepository = assignmentRepository;
        }

        private string GetTokenFromHeader()
        {
            var authHeader = Request.Headers["Authorization"];
            return authHeader.Count > 0 && authHeader[0].StartsWith("Bearer ")
                ? authHeader[0].Substring("Bearer ".Length).Trim()
                : null;
        }


        [HttpGet("{name}")]
        public ActionResult GetClassByName(string name)

        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing or invalid token");
            }
            string token = GetTokenFromHeader();
            // Retrieve the user based on the token.
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

        [HttpGet]
        //get all the classes with the teacher id
        public IActionResult GetClasses()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing or invalid token");
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();
            _token = token;
            // Retrieve the user based on the token.
            var user = _userRepository.GetUserByToken(token);

            if (user == null)
            {
                return Unauthorized("Invalid token");
            }

            var classes = _classRepository.GetClassesByTeacherId(user.Id);
            return Ok(classes);
        }

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

        [HttpGet("GetStudents")]

        public IActionResult GetStudents()
        {
            var students = _userRepository.GetAllStudents();
            return Ok(students);
        }


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




        [HttpPost("AddStudentToClass")]
        public IActionResult AddStudentToClass([FromBody] StudentClassRequest request)
        {
            var student = _userRepository.GetStudentByName(request.FirstName, request.LastName);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            string token = GetTokenFromHeader(); // Implement this method to get token from headers
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

        //GetAllStudentsNotInClass
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

        //get all students
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

        //RemoveStudentFromClass
        [HttpDelete("RemoveStudentFromClass")]
        public IActionResult RemoveStudentFromClass([FromQuery] string className,
                                                    [FromQuery] string firstName,
                                                    [FromQuery] string lastName)
        {
            // Get teacher ID from token
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
                return Unauthorized();

            string token = GetTokenFromHeader();
            var teacher = _userRepository.GetUserByToken(token);
            if (teacher == null) return Unauthorized("Invalid token");

            // Find the class
            var classObj = _classRepository.GetClassByNameAndTeacherId(className, teacher.Id);
            if (classObj == null) return NotFound("Class not found");

            // Find the student
            var student = _userRepository.GetStudentByName(firstName, lastName);
            if (student == null) return NotFound("Student not found");

            // Remove enrollment
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

        [HttpGet("{className}/assignments")]
        public IActionResult GetClassAssignments(string className)
        {
            string token = GetTokenFromHeader();
            var teacher = _userRepository.GetUserByToken(token);
            var classObj = _classRepository.GetClassByNameAndTeacherId(className, teacher.Id);


            var assignments = _assignmentRepository.GetAssignmentsByClassId(classObj.Id);
            return Ok(assignments);
        }

        [HttpPost("AddAssignment")]
        public IActionResult AddAssignment([FromBody] AssignmentResponse assignment)
        {
            string token = GetTokenFromHeader();
            var teacher = _userRepository.GetUserByToken(token);


            // Resolve class by name and teacher
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


        [HttpPut("UpdateAssignment/{id}")]
        public IActionResult UpdateAssignment(int id, [FromBody] Assignment assignment)
        {
            if (id != assignment.Id) return BadRequest();

            _assignmentRepository.Update(assignment);
            return NoContent();
        }

        [HttpDelete("DeleteAssignment/{id}")]
        public IActionResult DeleteAssignment(int id)
        {
            var assignment = _assignmentRepository.GetById(id);
            if (assignment == null) return NotFound();

            _assignmentRepository.Delete(assignment);
            return NoContent();
        }

    }

    public class StudentClassRequest
    {
        public string ClassName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AssignmentResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string ClassName { get; set; }
    }
}