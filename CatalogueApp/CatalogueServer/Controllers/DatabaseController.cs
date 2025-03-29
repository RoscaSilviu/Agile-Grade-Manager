using CatalogueServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class DatabaseController : ControllerBase
{
    private readonly Database _database;
    private readonly UserRepository _userRepository;
    private readonly AssignmentRepository _assignmentRepository;
    private readonly ClassRepository _classRepository;
    private readonly GradeRepository _gradeRepository;
    public DatabaseController(Database database)
    {
        _database = database;

    }

    // ========== USERS ==========
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _userRepository.GetAll();
        return Ok(users);
    }

    [HttpPost("users")]
    public IActionResult AddUser([FromBody] User user)
    {
        User newUser = new User
        {
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Password = PasswordHelper.HashPassword(user.Password),
            Role = user.Role
        };

        _userRepository.Insert(newUser);
        return Created($"api/users/{user.Id}", user);
    }

    [HttpDelete("users/{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _userRepository.GetById(id);
        if (user == null) return NotFound("User not found");

        _userRepository.Delete(user);
        return NoContent();
    }

    // ========== CLASSES ==========
    [HttpGet("classes")]
    public IActionResult GetClasses()
    {
        var classes = _classRepository.GetAll();
        return Ok(classes);
    }

    [HttpPost("classes")]
    public IActionResult AddClass([FromBody] Class newClass)
    {
        _classRepository.Insert(newClass);
        return Created($"api/classes/{newClass.Id}", newClass);
    }

    [HttpDelete("classes/{id}")]
    public IActionResult DeleteClass(int id)
    {
        var classObj = _classRepository.GetById(id);
        if (classObj == null) return NotFound("Class not found");

        _classRepository.Delete(classObj);
        return NoContent();
    }

    // ========== ASSIGNMENTS ==========
    [HttpGet("assignments")]
    public IActionResult GetAssignments()
    {
        var assignments = _assignmentRepository.GetAll();
        return Ok(assignments);
    }

    [HttpPost("assignments")]
    public IActionResult AddAssignment([FromBody] Assignment assignment)
    {
        _assignmentRepository.Insert(assignment);
        return Created($"api/assignments/{assignment.Id}", assignment);
    }

    [HttpDelete("assignments/{id}")]
    public IActionResult DeleteAssignment(int id)
    {
        var assignment = _assignmentRepository.GetById(id);
        if (assignment == null) return NotFound("Assignment not found");

        _assignmentRepository.Delete(assignment);
        return NoContent();
    }

    // ========== GRADES ==========
    [HttpGet("grades")]
    public IActionResult GetGrades()
    {
        var grades = _gradeRepository.GetAll();
        return Ok(grades);
    }

    [HttpPost("grades")]
    public IActionResult AddGrade([FromBody] Grade grade)
    {
        _gradeRepository.Insert(grade);
        return Created($"api/grades/{grade.Id}", grade);
    }

    [HttpDelete("grades/{id}")]
    public IActionResult DeleteGrade(int id)
    {
        var grade = _gradeRepository.GetById(id);
        if (grade == null) return NotFound("Grade not found");

        _gradeRepository.Delete(grade);
        return NoContent();
    }

    // ========== TEACHER-SPECIFIC ==========
    [HttpGet("teacher/{teacherId}/classes")]
    public IActionResult GetTeacherClasses(int teacherId)
    {
        var classes = _classRepository.GetClassesByTeacherId(teacherId);
        return Ok(classes);
    }

    [HttpGet("teacher/{teacherId}/assignments")]
    public IActionResult GetTeacherAssignments(int teacherId)
    {
        var assignments = _assignmentRepository.GetAssignmentsByTeacherId(teacherId);
        return Ok(assignments);
    }

    [HttpGet("teacher/{teacherId}/grades")]
    public IActionResult GetTeacherGrades(int teacherId)
    {
        var grades = _gradeRepository.GetGradesByTeacherId(teacherId);
        return Ok(grades);
    }

    // ========== STUDENT-SPECIFIC ==========
    [HttpGet("student/{studentId}/classes")]
    public IActionResult GetStudentClasses(int studentId)
    {
        var classes = _classRepository.GetStudentsInClass(studentId);
        return Ok(classes);
    }

    [HttpGet("student/{studentId}/assignments")]
    public IActionResult GetStudentAssignments(int studentId)
    {
        var assignments = _assignmentRepository.GetAssignmentsByClassId(studentId);
        return Ok(assignments);
    }

    [HttpGet("student/{studentId}/grades")]
    public IActionResult GetStudentGrades(int studentId)
    {
        var grades = _gradeRepository.GetGradesByStudentId(studentId); 
        return Ok(grades);
    }
}
