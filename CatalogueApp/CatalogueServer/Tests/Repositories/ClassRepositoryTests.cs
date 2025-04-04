using SQLite;
using Xunit;
using CatalogueServer.Repositories;

namespace CatalogueServer.Tests.Repositories
{
    public class ClassRepositoryTests : IDisposable
    {
        private readonly SQLiteConnection _db;
        private readonly ClassRepository _repository;

        public ClassRepositoryTests()
        {
            // Create in-memory database for testing
            _db = new SQLiteConnection(":memory:");

            // Create tables in correct order
            _db.Execute("PRAGMA foreign_keys = ON");
            _db.CreateTable<User>();
            _db.CreateTable<Class>();
            _db.CreateTable<ClassStudent>();

            var database = new Database(_db);
            _repository = new ClassRepository(database);
        }

        [Fact]
        public void GetClassesByTeacherId_ShouldReturnTeacherClasses()
        {
            // Arrange
            _db.DeleteAll<Class>();
            var teacherId = 1;
            var classes = new List<Class>
            {
                new Class { Id = 1, Name = "Math Class", TeacherId = teacherId },
                new Class { Id = 2, Name = "Science Class", TeacherId = teacherId },
                new Class { Id = 3, Name = "Other Class", TeacherId = 2 } // Different teacher
            };
            foreach (var cls in classes)
            {
                _db.Insert(cls);
            }

            // Act
            var result = _repository.GetClassesByTeacherId(teacherId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.Equal(teacherId, c.TeacherId));
        }

        [Fact]
        public void AddStudentToClass_ShouldAddStudent()
        {
            // Arrange
            _db.DeleteAll<ClassStudent>();
            var classId = 1;
            var studentId = 1;

            // Act
            _repository.AddStudentToClass(classId, studentId);

            // Assert
            var result = _db.Table<ClassStudent>()
                           .FirstOrDefault(cs => cs.ClassId == classId && cs.StudentId == studentId);
            Assert.NotNull(result);
            Assert.Equal(classId, result.ClassId);
            Assert.Equal(studentId, result.StudentId);
        }

        [Fact]
        public void RemoveStudentFromClass_ShouldRemoveStudent()
        {
            // Arrange
            _db.DeleteAll<ClassStudent>();
            var classId = 1;
            var studentId = 1;
            var classStudent = new ClassStudent { ClassId = classId, StudentId = studentId };
            _db.Insert(classStudent);

            // Act
            _repository.RemoveStudentFromClass(classId, studentId);

            // Assert
            var result = _db.Table<ClassStudent>()
                           .FirstOrDefault(cs => cs.ClassId == classId && cs.StudentId == studentId);
            Assert.Null(result);
        }

        [Fact]
        public void GetStudentsInClass_ShouldReturnAllStudents()
        {
            // Arrange
            _db.DeleteAll<ClassStudent>();
            _db.DeleteAll<User>();
            var classId = 1;
            var students = new List<User>
            {
                new User { Id = 1, Name = "Student 1", Role = "Student" },
                new User { Id = 2, Name = "Student 2", Role = "Student" }
            };
            foreach (var student in students)
            {
                _db.Insert(student);
                _db.Insert(new ClassStudent { ClassId = classId, StudentId = student.Id });
            }

            // Act
            var result = _repository.GetStudentsInClass(classId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(students[0].Id, result[0].Id);
            Assert.Equal(students[1].Id, result[1].Id);
        }

        [Fact]
        public void GetClassesByStudentId_ShouldReturnAllClasses()
        {
            // Arrange
            _db.DeleteAll<Class>();
            _db.DeleteAll<ClassStudent>();
            var studentId = 1;
            var classes = new List<Class>
            {
                new Class { Id = 1, Name = "Math Class", TeacherId = 1 },
                new Class { Id = 2, Name = "Science Class", TeacherId = 2 }
            };
            foreach (var cls in classes)
            {
                _db.Insert(cls);
                _db.Insert(new ClassStudent { ClassId = cls.Id, StudentId = studentId });
            }

            // Act
            var result = _repository.GetClassesByStudentId(studentId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(classes[0].Id, result[0].Id);
            Assert.Equal(classes[1].Id, result[1].Id);
        }

        [Fact]
        public void GetStudentsInClass_ShouldReturnEmptyList_WhenNoStudents()
        {
            // Arrange
            _db.DeleteAll<ClassStudent>();
            _db.DeleteAll<User>();
            var classId = 1;

            // Act
            var result = _repository.GetStudentsInClass(classId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void RemoveStudentFromClass_ShouldNotThrowException_WhenStudentNotInClass()
        {
            // Arrange
            _db.DeleteAll<ClassStudent>();
            var classId = 1;
            var studentId = 1;

            // Act & Assert
            var exception = Record.Exception(() => _repository.RemoveStudentFromClass(classId, studentId));
            Assert.Null(exception);
        }

        public void Dispose()
        {
            _db?.Close();
            _db?.Dispose();
        }
    }
}