using SQLite;
using Xunit;
using CatalogueServer.Repositories;

namespace CatalogueServer.Tests.Repositories
{
    public class GradeRepositoryTests : IDisposable
    {
        private readonly SQLiteConnection _db;
        private readonly GradeRepository _repository;

        public GradeRepositoryTests()
        {
            // Create in-memory database for testing
            _db = new SQLiteConnection(":memory:");

            // Create tables in correct order (respecting foreign keys)
            _db.Execute("PRAGMA foreign_keys = ON");
            _db.CreateTable<User>();
            _db.CreateTable<Class>();
            _db.CreateTable<Assignment>();
            _db.CreateTable<Grade>();

            var database = new Database(_db);
            _repository = new GradeRepository(database);
        }

        [Fact]
        public void GetGradesByTeacherId_ShouldReturnTeacherGrades()
        {
            // Arrange
            _db.DeleteAll<Grade>();
            _db.DeleteAll<Assignment>();
            _db.DeleteAll<Class>();

            var teacherId = 1;
            var classId = 1;
            var assignmentId = 1;

            // Insert test class
            _db.Insert(new Class { Id = classId, TeacherId = teacherId, Name = "Math" });

            // Insert test assignment
            _db.Insert(new Assignment { Id = assignmentId, ClassId = classId, Name = "Test 1" });

            // Insert test grades
            var grades = new List<Grade>
            {
                new Grade { StudentId = 1, AssignmentId = assignmentId, Value = 85, Date = DateTime.Now },
                new Grade { StudentId = 2, AssignmentId = assignmentId, Value = 90, Date = DateTime.Now }
            };
            foreach (var grade in grades)
            {
                _db.Insert(grade);
            }

            // Act
            var result = _repository.GetGradesByTeacherId(teacherId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, g => Assert.Equal(assignmentId, g.AssignmentId));
        }

        [Fact]
        public void GetGradesByStudentId_ShouldReturnStudentGrades()
        {
            // Arrange
            _db.DeleteAll<Grade>();
            var studentId = 1;
            var grades = new List<Grade>
            {
                new Grade { StudentId = studentId, AssignmentId = 1, Value = 85, Date = DateTime.Now },
                new Grade { StudentId = studentId, AssignmentId = 2, Value = 90, Date = DateTime.Now }
            };
            foreach (var grade in grades)
            {
                _db.Insert(grade);
            }

            // Act
            var result = _repository.GetGradesByStudentId(studentId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, g => Assert.Equal(studentId, g.StudentId));
        }

        [Fact]
        public void GetGradesByAssignmentId_ShouldReturnAssignmentGrades()
        {
            // Arrange
            _db.DeleteAll<Grade>();
            var assignmentId = 1;
            var grades = new List<Grade>
            {
                new Grade { StudentId = 1, AssignmentId = assignmentId, Value = 85, Date = DateTime.Now },
                new Grade { StudentId = 2, AssignmentId = assignmentId, Value = 90, Date = DateTime.Now }
            };
            foreach (var grade in grades)
            {
                _db.Insert(grade);
            }

            // Act
            var result = _repository.GetGradesByAssignmentId(assignmentId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, g => Assert.Equal(assignmentId, g.AssignmentId));
        }

        [Fact]
        public void GradeAssignment_ShouldInsertGrade()
        {
            // Arrange
            _db.DeleteAll<Grade>();
            var studentId = 1;
            var assignmentId = 1;
            var gradeValue = 85;

            // Act
            _repository.GradeAssignment(studentId, assignmentId, gradeValue);

            // Assert
            var result = _db.Table<Grade>().FirstOrDefault(g =>
                g.StudentId == studentId &&
                g.AssignmentId == assignmentId);

            Assert.NotNull(result);
            Assert.Equal(gradeValue, result.Value);
        }

        [Fact]
        public void BulkGradeAssignments_ShouldInsertMultipleGrades()
        {
            // Arrange
            _db.DeleteAll<Grade>();
            var grades = new List<Grade>
            {
                new Grade { StudentId = 1, AssignmentId = 1, Value = 85, Date = DateTime.Now },
                new Grade { StudentId = 2, AssignmentId = 1, Value = 90, Date = DateTime.Now }
            };

            // Act
            _repository.BulkGradeAssignments(grades);

            // Assert
            var result = _db.Table<Grade>().ToList();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetStudentAverageGrades_ShouldReturnAveragesPerSubject()
        {
            // Arrange
            _db.DeleteAll<Grade>();
            _db.DeleteAll<Assignment>();
            _db.DeleteAll<Class>();

            var studentId = 1;
            var mathClassId = 1;
            var scienceClassId = 2;

            // Insert classes
            _db.Insert(new Class { Id = mathClassId, Name = "Math" });
            _db.Insert(new Class { Id = scienceClassId, Name = "Science" });

            // Insert assignments
            _db.Insert(new Assignment { Id = 1, ClassId = mathClassId, Name = "Math Test 1" });
            _db.Insert(new Assignment { Id = 2, ClassId = mathClassId, Name = "Math Test 2" });
            _db.Insert(new Assignment { Id = 3, ClassId = scienceClassId, Name = "Science Test" });

            // Insert grades
            var grades = new List<Grade>
            {
                new Grade { StudentId = studentId, AssignmentId = 1, Value = 80, Date = DateTime.Now.AddDays(-2) },
                new Grade { StudentId = studentId, AssignmentId = 2, Value = 90, Date = DateTime.Now },
                new Grade { StudentId = studentId, AssignmentId = 3, Value = 85, Date = DateTime.Now.AddDays(-1) }
            };
            foreach (var grade in grades)
            {
                _db.Insert(grade);
            }

            // Act
            var result = _repository.GetStudentAverageGrades(studentId);

            // Assert
            Assert.Equal(2, result.Count); // Should have 2 subjects
            var mathGrades = result.First(g => g.Subject == "Math");
            Assert.Equal(85, mathGrades.AverageGrade); // (80 + 90) / 2
        }

        [Fact]
        public void GetStudentGradesBySubject_ShouldReturnDetailedGrades()
        {
            // Arrange
            _db.DeleteAll<Grade>();
            _db.DeleteAll<Assignment>();
            _db.DeleteAll<Class>();

            var studentId = 1;
            var subject = "Math";
            var classId = 1;

            // Insert class
            _db.Insert(new Class { Id = classId, Name = subject });

            // Insert assignments
            _db.Insert(new Assignment
            {
                Id = 1,
                ClassId = classId,
                Name = "Math Test 1",
                Description = "Chapter 1 Test"
            });

            // Insert grade
            _db.Insert(new Grade
            {
                StudentId = studentId,
                AssignmentId = 1,
                Value = 85,
                Date = DateTime.Now
            });

            // Act
            var result = _repository.GetStudentGradesBySubject(studentId, subject);

            // Assert
            Assert.Single(result);
            var grade = result[0];
            Assert.Equal(subject, grade.Subject);
            Assert.Equal(85, grade.Value);
            Assert.Equal("Math Test 1", grade.AssignmentName);
            Assert.Equal("Chapter 1 Test", grade.Comments);
        }

        public void Dispose()
        {
            _db?.Close();
            _db?.Dispose();
        }
    }
}