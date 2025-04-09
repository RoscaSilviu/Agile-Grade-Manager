using System.Linq;
using CatalogueServer.Controllers;
using SQLite;


namespace CatalogueServer.Repositories
{
    /// <summary>
    /// Repository implementation for managing grades and grade-related operations.
    /// </summary>
    public class GradeRepository : BaseRepository<Grade>, IGradeRepository
    {
        /// <summary>
        /// Initializes a new instance of the GradeRepository class.
        /// </summary>
        /// <param name="database">The database instance containing the connection.</param>
        public GradeRepository(Database database) : base(database.Connection) { }

        // Get all grades for a teacher
        public List<Grade> GetGradesByTeacherId(int teacherId)
        {
            var teacherClasses = _db.Table<Class>().Where(c => c.TeacherId == teacherId).Select(c => c.Id).ToList();
            var assignments = _db.Table<Assignment>().Where(a => teacherClasses.Contains(a.ClassId)).Select(a => a.Id).ToList();
            return _db.Table<Grade>().Where(g => assignments.Contains(g.AssignmentId)).ToList();
        }

        // Get all grades for a student
        public List<Grade> GetGradesByStudentId(int studentId)
        {
            return _db.Table<Grade>().Where(g => g.StudentId == studentId).ToList();
        }

        // Get all grades for a specific assignment
        public List<Grade> GetGradesByAssignmentId(int assignmentId)
        {
            return _db.Table<Grade>().Where(g => g.AssignmentId == assignmentId).ToList();
        }

        // Grade a single assignment
        public void GradeAssignment(int studentId, int assignmentId, int gradeValue)
        {
            var grade = new Grade
            {
                StudentId = studentId,
                AssignmentId = assignmentId,
                Value = gradeValue,
                Date = DateTime.Now
            };
            Insert(grade);
        }

        // Bulk grade assignments
        public void BulkGradeAssignments(List<Grade> grades)
        {
            _db.InsertAll(grades);
        }

        public List<(string Subject, double AverageGrade, DateTime LastGraded)> GetStudentAverageGrades(int studentId)
        {
            var grades = GetGradesByStudentId(studentId);

            // Join with assignments to get subject information
            var query = from g in grades
                        join a in _db.Table<Assignment>()
                        on g.AssignmentId equals a.Id
                        join c in _db.Table<Class>()
                        on a.ClassId equals c.Id
                        group new { g.Value, g.Date } by c.Name into subjectGroup
                        select new
                        {
                            Subject = subjectGroup.Key,
                            AverageGrade = subjectGroup.Average(x => x.Value),
                            LastGraded = subjectGroup.Max(x => x.Date)
                        };

            return query.Select(x => (x.Subject, x.AverageGrade, x.LastGraded)).ToList();
        }

        public List<GradeDetail> GetStudentGradesBySubject(int studentId, string subject)
        {
            var query = from g in _db.Table<Grade>()
                        join a in _db.Table<Assignment>()
                        on g.AssignmentId equals a.Id
                        join c in _db.Table<Class>()
                        on a.ClassId equals c.Id
                        where g.StudentId == studentId && c.Name == subject
                        select new GradeDetail
                        {
                            Subject = c.Name,
                            Value = g.Value,
                            Date = g.Date,
                            AssignmentName = a.Name,
                            Comments = a.Description
                        };

            return query.ToList();
        }

        /// <summary>
        /// Retrieves detailed grade information for a teacher's classes.
        /// </summary>
        /// <param name="teacherId">The ID of the teacher.</param>
        /// <returns>A list of detailed grade information for the teacher's classes.</returns>
        public List<TeacherGradeDetail> GetTeacherGradeDetails(int teacherId)
        {
            var query = from g in _db.Table<Grade>()
                        join a in _db.Table<Assignment>() on g.AssignmentId equals a.Id
                        join c in _db.Table<Class>() on a.ClassId equals c.Id
                        join s in _db.Table<User>() on g.StudentId equals s.Id
                        where c.TeacherId == teacherId
                        select new TeacherGradeDetail
                        {
                            GradeId = g.Id,
                            StudentName = $"{s.Name} {s.Surname}",
                            AssignmentName = a.Name,
                            Value = g.Value,
                            Date = g.Date,
                            Comments = a.Description
                        };

            return query.ToList();
        }

        public Assignment GetAssignmentByNameAndTeacher(string assignmentName, int teacherId)
        {
            return _db.Table<Assignment>()
                .Join(_db.Table<Class>(),
                    a => a.ClassId,
                    c => c.Id,
                    (a, c) => new { Assignment = a, Class = c })
                .Where(ac => ac.Class.TeacherId == teacherId && ac.Assignment.Name == assignmentName)
                .Select(ac => ac.Assignment)
                .FirstOrDefault();
        }

        /// <summary>
        /// Updates the grade value for a specific grade entry.
        /// </summary>
        /// <param name="gradeId">The ID of the grade to update.</param>
        /// <param name="newValue">The new grade value to assign.</param>
        public void UpdateGrade(int gradeId, int newValue)
        {
            var grade = _db.Table<Grade>().FirstOrDefault(g => g.Id == gradeId);
            if (grade != null)
            {
                grade.Value = newValue;
                grade.Date = DateTime.Now; // Update the date to reflect the change
                Update(grade);
            }
        }

        /// <summary>
        /// Deletes a specific grade entry from the database.
        /// </summary>
        /// <param name="gradeId">The ID of the grade to delete.</param>
        public void DeleteGrade(int gradeId)
        {
            var grade = _db.Table<Grade>().FirstOrDefault(g => g.Id == gradeId);
            if (grade != null)
            {
                Delete(grade);
            }
        }

        /// <summary>
        /// Represents detailed information about a student's grade.
        /// </summary>
        public class GradeDetail
        {
            /// <summary>
            /// Gets or sets the subject name.
            /// </summary>
            public string Subject { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the grade value.
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// Gets or sets the date when the grade was assigned.
            /// </summary>
            public DateTime Date { get; set; }

            /// <summary>
            /// Gets or sets the name of the assignment.
            /// </summary>
            public string AssignmentName { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the name of the teacher.
            /// </summary>
            public string TeacherName { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets any comments associated with the grade.
            /// </summary>
            public string? Comments { get; set; }
        }
    }
}
