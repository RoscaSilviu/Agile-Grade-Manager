using SQLite;
using Xunit;
using CatalogueServer.Repositories;

public class AssignmentRepositoryTests : IDisposable
{
    private readonly SQLiteConnection _db;
    private readonly AssignmentRepository _repository;

    public AssignmentRepositoryTests()
    {
        // Create in-memory database for testing
        _db = new SQLiteConnection(":memory:");

        // Create tables in correct order (respecting foreign key relationships)
        _db.Execute("PRAGMA foreign_keys = ON");
        _db.CreateTable<User>();
        _db.CreateTable<Class>();
        _db.CreateTable<Grade>();
        _db.CreateTable<Assignment>();

        var database = new Database(_db);
        _repository = new AssignmentRepository(database);
    }

    [Fact]
    public void GetAssignmentsByClassId_ShouldReturnAssignmentsForClass()
    {
        // Clear any existing data
        _db.DeleteAll<Assignment>();
        _db.DeleteAll<Class>();

        // Arrange
        var classId = 1;
        var assignments = new List<Assignment>
        {
            new Assignment { Id = 1, ClassId = classId, Name = "Assignment 1" },
            new Assignment { Id = 2, ClassId = classId, Name = "Assignment 2" }
        };

        // Insert test data
        foreach (var assignment in assignments)
        {
            _db.Insert(assignment);
        }

        // Act
        var result = _repository.GetAssignmentsByClassId(classId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Equal(classId, a.ClassId));
    }

    [Fact]
    public void GetAssignmentsByTeacherId_ShouldReturnAssignmentsForTeacher()
    {
        // Clear any existing data
        _db.DeleteAll<Assignment>();
        _db.DeleteAll<Class>();

        // Arrange
        var teacherId = 1;
        var classId = 1;

        // Insert test class
        var testClass = new Class { Id = classId, TeacherId = teacherId };
        _db.Insert(testClass);

        // Insert test assignments
        var assignments = new List<Assignment>
        {
            new Assignment { Id = 1, ClassId = classId, Name = "Assignment 1" },
            new Assignment { Id = 2, ClassId = classId, Name = "Assignment 2" }
        };
        foreach (var assignment in assignments)
        {
            _db.Insert(assignment);
        }

        // Act
        var result = _repository.GetAssignmentsByTeacherId(teacherId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Equal(classId, a.ClassId));
    }

    public void Dispose()
    {
        _db?.Close();
        _db?.Dispose();
    }
}