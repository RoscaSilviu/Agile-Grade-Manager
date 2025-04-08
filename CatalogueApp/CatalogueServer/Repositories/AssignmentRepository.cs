using CatalogueServer.Repositories;

/// <summary>
/// Repository implementation for managing assignments in the school management system.
/// </summary>
public class AssignmentRepository : BaseRepository<Assignment>
{
    /// <summary>
    /// Initializes a new instance of the AssignmentRepository class.
    /// </summary>
    /// <param name="database">The database instance containing the connection.</param>
    public AssignmentRepository(Database database) : base(database.Connection) { }

    /// <summary>
    /// Retrieves all assignments associated with a specific class.
    /// </summary>
    /// <param name="classId">The ID of the class to get assignments for.</param>
    /// <returns>A list of assignments belonging to the specified class.</returns>
    public List<Assignment> GetAssignmentsByClassId(int classId)
    {
        return _db.Table<Assignment>().Where(a => a.ClassId == classId).ToList();
    }

    /// <summary>
    /// Retrieves all assignments for a specific teacher across all their classes.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <returns>A list of all assignments in classes taught by the specified teacher.</returns>
    /// <remarks>
    /// This method performs a two-step query:
    /// 1. Gets all classes taught by the teacher
    /// 2. Gets all assignments associated with those classes
    /// </remarks>
    public List<Assignment> GetAssignmentsByTeacherId(int teacherId)
    {
        var teacherClasses = _db.Table<Class>().Where(c => c.TeacherId == teacherId).Select(c => c.Id).ToList();
        return _db.Table<Assignment>().Where(a => teacherClasses.Contains(a.ClassId)).ToList();
    }
}