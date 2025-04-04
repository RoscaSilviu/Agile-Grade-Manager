using SQLite;
using Xunit;
using CatalogueServer.Repositories;

namespace CatalogueServer.Tests.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly SQLiteConnection _db;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            // Create in-memory database for testing
            _db = new SQLiteConnection(":memory:");

            // Create tables in correct order
            _db.Execute("PRAGMA foreign_keys = ON");
            _db.CreateTable<User>();
            _db.CreateTable<Class>();
            _db.CreateTable<ClassStudent>();

            var database = new Database(_db);
            _repository = new UserRepository(database);
        }

        [Fact]
        public void GetUserByEmail_ShouldReturnUser_WhenExists()
        {
            // Arrange
            _db.DeleteAll<User>();
            var testUser = new User
            {
                Email = "test@example.com",
                Name = "Test",
                Surname = "User",
                Password = "password",
                Role = "Student"
            };
            _db.Insert(testUser);

            // Act
            var result = _repository.GetUserByEmail("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testUser.Email, result.Email);
            Assert.Equal(testUser.Name, result.Name);
        }

        [Fact]
        public void GetUserByEmail_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _db.DeleteAll<User>();

            // Act
            var result = _repository.GetUserByEmail("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetStudentsByTeacherId_ShouldReturnAllStudents()
        {
            // Arrange
            _db.DeleteAll<User>();
            _db.DeleteAll<Class>();
            _db.DeleteAll<ClassStudent>();

            var teacherId = 1;
            var classId = 1;

            // Create test class
            var testClass = new Class { Id = classId, TeacherId = teacherId, Name = "Test Class" };
            _db.Insert(testClass);

            // Create test students
            var students = new List<User>
            {
                new User { Id = 1, Name = "Student 1", Email = "student1@test.com", Role = "Student" },
                new User { Id = 2, Name = "Student 2", Email = "student2@test.com", Role = "Student" }
            };
            foreach (var student in students)
            {
                _db.Insert(student);
                _db.Insert(new ClassStudent { ClassId = classId, StudentId = student.Id });
            }

            // Act
            var result = _repository.GetStudentsByTeacherId(teacherId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Email == "student1@test.com");
            Assert.Contains(result, u => u.Email == "student2@test.com");
        }

        [Fact]
        public void GetStudentsByTeacherId_ShouldReturnDistinctStudents()
        {
            // Arrange
            _db.DeleteAll<User>();
            _db.DeleteAll<Class>();
            _db.DeleteAll<ClassStudent>();

            var teacherId = 1;
            var class1Id = 1;
            var class2Id = 2;

            // Create test classes
            _db.Insert(new Class { Id = class1Id, TeacherId = teacherId, Name = "Class 1" });
            _db.Insert(new Class { Id = class2Id, TeacherId = teacherId, Name = "Class 2" });

            // Create test student (in both classes)
            var student = new User { Id = 1, Name = "Student 1", Email = "student1@test.com", Role = "Student" };
            _db.Insert(student);

            // Add student to both classes
            _db.Insert(new ClassStudent { ClassId = class1Id, StudentId = student.Id });
            _db.Insert(new ClassStudent { ClassId = class2Id, StudentId = student.Id });

            // Act
            var result = _repository.GetStudentsByTeacherId(teacherId);

            // Assert
            Assert.Single(result); // Should only return one student despite being in two classes
            Assert.Equal(student.Email, result[0].Email);
        }

        [Fact]
        public void GetUserByToken_ShouldReturnUser_WhenExists()
        {
            // Arrange
            _db.DeleteAll<User>();
            var testUser = new User
            {
                Email = "test@example.com",
                Name = "Test",
                Surname = "User",
                Token = "test-token-123"
            };
            _db.Insert(testUser);

            // Act
            var result = _repository.GetUserByToken("test-token-123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testUser.Token, result.Token);
            Assert.Equal(testUser.Email, result.Email);
        }

        [Fact]
        public void GetUserByToken_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _db.DeleteAll<User>();

            // Act
            var result = _repository.GetUserByToken("nonexistent-token");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Insert_ShouldAddNewUser()
        {
            // Arrange
            _db.DeleteAll<User>();
            var newUser = new User
            {
                Email = "new@example.com",
                Name = "New",
                Surname = "User",
                Password = "password",
                Role = "Student"
            };

            // Act
            _repository.Insert(newUser);

            // Assert
            var result = _db.Table<User>().FirstOrDefault(u => u.Email == "new@example.com");
            Assert.NotNull(result);
            Assert.Equal(newUser.Email, result.Email);
            Assert.Equal(newUser.Name, result.Name);
        }

        [Fact]
        public void Update_ShouldModifyExistingUser()
        {
            // Arrange
            _db.DeleteAll<User>();
            var user = new User
            {
                Email = "test@example.com",
                Name = "Test",
                Surname = "User",
                Password = "password"
            };
            _db.Insert(user);

            // Modify user
            user.Name = "Updated";

            // Act
            _repository.Update(user);

            // Assert
            var result = _db.Table<User>().First(u => u.Email == "test@example.com");
            Assert.Equal("Updated", result.Name);
        }

        public void Dispose()
        {
            _db?.Close();
            _db?.Dispose();
        }
    }
}