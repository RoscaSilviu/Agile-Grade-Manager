using CatalogueServer.Repositories;

/// <summary>
/// Interface defining operations for managing grades in the system.
/// </summary>
public interface IGradeRepository
{
    /// <summary>
    /// Retrieves all grades associated with a specific teacher.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <returns>A list of grades for assignments in the teacher's classes.</returns>
    List<Grade> GetGradesByTeacherId(int teacherId);

    /// <summary>
    /// Retrieves all grades for a specific student.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <returns>A list of the student's grades.</returns>
    List<Grade> GetGradesByStudentId(int studentId);

    /// <summary>
    /// Retrieves all grades for a specific assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <returns>A list of grades for the assignment.</returns>
    List<Grade> GetGradesByAssignmentId(int assignmentId);

    /// <summary>
    /// Records a grade for a student's assignment.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <param name="gradeValue">The grade value to record.</param>
    void GradeAssignment(int studentId, int assignmentId, int gradeValue);

    /// <summary>
    /// Records multiple grades at once.
    /// </summary>
    /// <param name="grades">The list of grades to record.</param>
    void BulkGradeAssignments(List<Grade> grades);

    /// <summary>
    /// Calculates average grades per subject for a student.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <returns>A list of tuples containing subject name, average grade, and last graded date.</returns>
    List<(string Subject, double AverageGrade, DateTime LastGraded)> GetStudentAverageGrades(int studentId);

    /// <summary>
    /// Retrieves detailed grade information for a student in a specific subject.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="subject">The name of the subject.</param>
    /// <returns>A list of detailed grade information.</returns>
    List<GradeRepository.GradeDetail> GetStudentGradesBySubject(int studentId, string subject);
}