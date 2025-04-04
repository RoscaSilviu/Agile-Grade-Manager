﻿using System.Linq;
using SQLite;


namespace CatalogueServer.Repositories
{
    public class GradeRepository : BaseRepository<Grade>
    {
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
    }
}
