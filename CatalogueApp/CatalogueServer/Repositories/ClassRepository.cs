using CatalogueServer.Repositories;

/// <summary>
/// Repository implementation for managing classes and student enrollments.
/// </summary>
public class ClassRepository : BaseRepository<Class>
{
    /// <summary>
    /// Initializes a new instance of the ClassRepository class.
    /// </summary>
    /// <param name="database">The database instance containing the connection.</param>
    public ClassRepository(Database database) : base(database.Connection) { }

    /// <summary>
    /// Retrieves all classes taught by a specific teacher.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <returns>A list of classes assigned to the specified teacher.</returns>
    public List<Class> GetClassesByTeacherId(int teacherId)
    {
        return _db.Table<Class>().Where(c => c.TeacherId == teacherId).ToList();
    }

    /// <summary>
    /// Enrolls a student in a specific class.
    /// </summary>
    /// <param name="classId">The ID of the class.</param>
    /// <param name="studentId">The ID of the student to enroll.</param>
    public void AddStudentToClass(int classId, int studentId)
    {
        var classStudent = new ClassStudent { ClassId = classId, StudentId = studentId };
        _db.Insert(classStudent);
    }

    /// <summary>
    /// Removes a student's enrollment from a specific class.
    /// </summary>
    /// <param name="classId">The ID of the class.</param>
    /// <param name="studentId">The ID of the student to remove.</param>
    public void RemoveStudentFromClass(int classId, int studentId)
    {
        var classStudent = _db.Table<ClassStudent>()
                              .FirstOrDefault(cs => cs.ClassId == classId && cs.StudentId == studentId);
        if (classStudent != null)
        {
            _db.Delete(classStudent);
        }
    }

    /// <summary>
    /// Retrieves all students enrolled in a specific class.
    /// </summary>
    /// <param name="classId">The ID of the class.</param>
    /// <returns>A list of users who are enrolled in the specified class.</returns>
    public List<User> GetStudentsInClass(int classId)
    {
        var studentIds = _db.Table<ClassStudent>()
                            .Where(cs => cs.ClassId == classId)
                            .Select(cs => cs.StudentId)
                            .ToList();

        return _db.Table<User>().Where(u => studentIds.Contains(u.Id)).ToList();
    }

    /// <summary>
    /// Retrieves all classes in which a specific student is enrolled.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <returns>A list of classes the student is enrolled in.</returns>
    public List<Class> GetClassesByStudentId(int studentId)
    {
        return (from c in _db.Table<Class>()
                join cs in _db.Table<ClassStudent>() on c.Id equals cs.ClassId
                where cs.StudentId == studentId
                select c).ToList();
    }

    /// <summary>
    /// Retrieves a specific class by its name and teacher.
    /// </summary>
    /// <param name="name">The name of the class.</param>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <returns>The matching class if found; null otherwise.</returns>
    public Class GetClassByNameAndTeacherId(string name, int teacherId)
    {
        return _db.Table<Class>().FirstOrDefault(c => c.Name == name && c.TeacherId == teacherId);
    }
}