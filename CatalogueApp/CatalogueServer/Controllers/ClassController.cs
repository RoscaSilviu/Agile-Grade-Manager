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

    }

}
