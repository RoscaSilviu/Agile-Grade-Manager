using Microsoft.AspNetCore.Mvc;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class DatabaseController : ControllerBase
{
    private readonly Database _database;

    public DatabaseController(Database database)
    {
        _database = database;
    }

    [HttpGet("test")]
    public IActionResult TestDatabase()
    {
        var db = _database.GetConnection();

        var teacher = new User
        {
            Password = "hashed_password",
            Email = "john@school.com",
            Role = "Teacher",
            Surname = "Doe",
            Name = "John",
            Token = "token",
            LastLogin = DateTime.Now
        };

        db.Insert(teacher);

        var mathClass = new Class
        {
            Name = "Mathematics",
            TeacherId = teacher.Id
        };

        db.Insert(mathClass);

        var classes = db.Table<Class>().ToList();

        return Ok(classes);
    }


    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var db = _database.GetConnection();
        var users = db.Table<User>().ToList();
        return Ok(users);
    }

    [HttpGet("classes")]
    public IActionResult GetClasses()
    {
        var db = _database.GetConnection();
        var classes = db.Table<Class>().ToList();
        return Ok(classes);
    }

    [HttpGet("grades")]
    public IActionResult GetGrades()
    {
        var db = _database.GetConnection();
        var grades = db.Table<Grade>().ToList();
        return Ok(grades);
    }

    [HttpGet("assignments")]
    public IActionResult GetAssignments()
    {
        var db = _database.GetConnection();
        var assignments = db.Table<Assignment>().ToList();
        return Ok(assignments);
    }

}
