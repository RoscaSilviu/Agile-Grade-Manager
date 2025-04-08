using CatalogueServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

/// <summary>
/// Controller responsible for managing core database operations in the school management system.
/// Provides administrative endpoints for managing users, classes, assignments, and grades.
/// </summary>
/// <remarks>
/// This controller handles system-wide operations and should be accessed with appropriate authorization.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class DatabaseController : ControllerBase
{
    private readonly Database _database;
    private readonly UserRepository _userRepository;
    private readonly AssignmentRepository _assignmentRepository;
    private readonly ClassRepository _classRepository;
    private readonly GradeRepository _gradeRepository;

    /// <summary>
    /// Initializes a new instance of the DatabaseController.
    /// </summary>
    /// <param name="database">The database instance for data operations.</param>
    public DatabaseController(Database database)
    {
        _database = database;
        _userRepository = new UserRepository(database);
        _assignmentRepository = new AssignmentRepository(database);
        _classRepository = new ClassRepository(database);
        _gradeRepository = new GradeRepository(database);
    }

    // ========== USERS ==========
    /// <summary>
    /// Retrieves all users in the system.
    /// </summary>
    /// <returns>200 OK with list of all users.</returns>
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _userRepository.GetAll();
        return Ok(users);
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="user">The user information to create.</param>
    /// <returns>201 Created with the new user details.</returns>
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

    /// <summary>
    /// Deletes a user from the system.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>204 NoContent if deleted; 404 NotFound if user doesn't exist.</returns>
    [HttpDelete("users/{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _userRepository.GetById(id);
        if (user == null) return NotFound("User not found");

        _userRepository.Delete(user);
        return NoContent();
    }

    // ========== CLASSES ==========
    /// <summary>
    /// Retrieves all classes in the system.
    /// </summary>
    /// <returns>200 OK with list of all classes.</returns>
    [HttpGet("classes")]
    public IActionResult GetClasses()
    {
        var classes = _classRepository.GetAll();
        return Ok(classes);
    }

    /// <summary>
    /// Creates a new class in the system.
    /// </summary>
    /// <param name="newClass">The class information to create.</param>
    /// <returns>201 Created with the new class details.</returns>
    [HttpPost("classes")]
    public IActionResult AddClass([FromBody] Class newClass)
    {
        _classRepository.Insert(newClass);
        return Created($"api/classes/{newClass.Id}", newClass);
    }

    /// <summary>
    /// Deletes a class from the system.
    /// </summary>
    /// <param name="id">The ID of the class to delete.</param>
    /// <returns>204 NoContent if deleted; 404 NotFound if class doesn't exist.</returns>
    [HttpDelete("classes/{id}")]
    public IActionResult DeleteClass(int id)
    {
        var classObj = _classRepository.GetById(id);
        if (classObj == null) return NotFound("Class not found");

        _classRepository.Delete(classObj);
        return NoContent();
    }

    // ========== ASSIGNMENTS ==========
    /// <summary>
    /// Retrieves all assignments in the system.
    /// </summary>
    /// <returns>200 OK with list of all assignments.</returns>
    [HttpGet("assignments")]
    public IActionResult GetAssignments()
    {
        var assignments = _assignmentRepository.GetAll();
        return Ok(assignments);
    }

    /// <summary>
    /// Creates a new assignment in the system.
    /// </summary>
    /// <param name="assignment">The assignment information to create.</param>
    /// <returns>201 Created with the new assignment details.</returns>
    [HttpPost("assignments")]
    public IActionResult AddAssignment([FromBody] Assignment assignment)
    {
        _assignmentRepository.Insert(assignment);
        return Created($"api/assignments/{assignment.Id}", assignment);
    }

    /// <summary>
    /// Deletes an assignment from the system.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    /// <returns>204 NoContent if deleted; 404 NotFound if assignment doesn't exist.</returns>
    [HttpDelete("assignments/{id}")]
    public IActionResult DeleteAssignment(int id)
    {
        var assignment = _assignmentRepository.GetById(id);
        if (assignment == null) return NotFound("Assignment not found");

        _assignmentRepository.Delete(assignment);
        return NoContent();
    }

    // ========== GRADES ==========
    /// <summary>
    /// Retrieves all grades in the system.
    /// </summary>
    /// <returns>200 OK with list of all grades.</returns>
    [HttpGet("grades")]
    public IActionResult GetGrades()
    {
        var grades = _gradeRepository.GetAll();
        return Ok(grades);
    }

    /// <summary>
    /// Creates a new grade in the system.
    /// </summary>
    /// <param name="grade">The grade information to create.</param>
    /// <returns>201 Created with the new grade details.</returns>
    [HttpPost("grades")]
    public IActionResult AddGrade([FromBody] Grade grade)
    {
        _gradeRepository.Insert(grade);
        return Created($"api/grades/{grade.Id}", grade);
    }

    /// <summary>
    /// Deletes a grade from the system.
    /// </summary>
    /// <param name="id">The ID of the grade to delete.</param>
    /// <returns>204 NoContent if deleted; 404 NotFound if grade doesn't exist.</returns>
    [HttpDelete("grades/{id}")]
    public IActionResult DeleteGrade(int id)
    {
        var grade = _gradeRepository.GetById(id);
        if (grade == null) return NotFound("Grade not found");

        _gradeRepository.Delete(grade);
        return NoContent();
    }

    // ========== TEACHER-SPECIFIC ==========
    /// <summary>
    /// Retrieves all classes for a specific teacher.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <returns>200 OK with list of teacher's classes.</returns>
    [HttpGet("teacher/{teacherId}/classes")]
    public IActionResult GetTeacherClasses(int teacherId)
    {
        var classes = _classRepository.GetClassesByTeacherId(teacherId);
        return Ok(classes);
    }

    /// <summary>
    /// Retrieves all assignments for a specific teacher.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <returns>200 OK with list of teacher's assignments.</returns>
    [HttpGet("teacher/{teacherId}/assignments")]
    public IActionResult GetTeacherAssignments(int teacherId)
    {
        var assignments = _assignmentRepository.GetAssignmentsByTeacherId(teacherId);
        return Ok(assignments);
    }

    /// <summary>
    /// Retrieves all grades given by a specific teacher.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <returns>200 OK with list of grades given by the teacher.</returns>
    [HttpGet("teacher/{teacherId}/grades")]
    public IActionResult GetTeacherGrades(int teacherId)
    {
        var grades = _gradeRepository.GetGradesByTeacherId(teacherId);
        return Ok(grades);
    }

    // ========== STUDENT-SPECIFIC ==========
    /// <summary>
    /// Retrieves all classes for a specific student.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <returns>200 OK with list of student's classes.</returns>
    [HttpGet("student/{studentId}/classes")]
    public IActionResult GetStudentClasses(int studentId)
    {
        var classes = _classRepository.GetStudentsInClass(studentId);
        return Ok(classes);
    }

    /// <summary>
    /// Retrieves all assignments for a specific student.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <returns>200 OK with list of student's assignments.</returns>
    [HttpGet("student/{studentId}/assignments")]
    public IActionResult GetStudentAssignments(int studentId)
    {
        var assignments = _assignmentRepository.GetAssignmentsByClassId(studentId);
        return Ok(assignments);
    }

    /// <summary>
    /// Retrieves all grades for a specific student.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <returns>200 OK with list of student's grades.</returns>
    [HttpGet("student/{studentId}/grades")]
    public IActionResult GetStudentGrades(int studentId)
    {
        var grades = _gradeRepository.GetGradesByStudentId(studentId);
        return Ok(grades);
    }

    /// <summary>
    /// Retrieves average grades for a specific student across all subjects.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <returns>200 OK with list of student's average grades by subject.</returns>
    [HttpGet("student/{studentId}/averagegrades")]
    public IActionResult GetStudentAverageGrades(int studentId)
    {
        var averageGrades = _gradeRepository.GetStudentAverageGrades(studentId);
        return Ok(averageGrades);
    }
}