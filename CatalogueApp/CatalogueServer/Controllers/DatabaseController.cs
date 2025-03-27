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
}
