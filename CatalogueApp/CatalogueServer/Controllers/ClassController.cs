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

        private string _token;

        public ClassController(ClassRepository classRepository, UserRepository userRepository)
        {
            _classRepository = classRepository;
            _userRepository = userRepository;

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

        private string GetTokenFromHeader()
        {
            var authHeader = Request.Headers["Authorization"];
            return authHeader.Count > 0 && authHeader[0].StartsWith("Bearer ")
                ? authHeader[0].Substring("Bearer ".Length).Trim()
                : null;
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

    }

    public class StudentClassRequest
    {
        public string ClassName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}
