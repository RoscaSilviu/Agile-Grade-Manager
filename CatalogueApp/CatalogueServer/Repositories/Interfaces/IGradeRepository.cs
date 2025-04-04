using CatalogueServer.Repositories;

public interface IGradeRepository
{
    List<Grade> GetGradesByTeacherId(int teacherId);
    List<Grade> GetGradesByStudentId(int studentId);
    List<Grade> GetGradesByAssignmentId(int assignmentId);
    void GradeAssignment(int studentId, int assignmentId, int gradeValue);
    void BulkGradeAssignments(List<Grade> grades);
    List<(string Subject, double AverageGrade, DateTime LastGraded)> GetStudentAverageGrades(int studentId);
    List<GradeRepository.GradeDetail> GetStudentGradesBySubject(int studentId, string subject);
}