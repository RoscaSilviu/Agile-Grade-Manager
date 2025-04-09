using CatalogueServer.Repositories;

/// <summary>
/// Repository implementation for managing users and user-related operations.
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="database">The database instance containing the connection.</param>
    public UserRepository(Database database) : base(database.Connection) { }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The matching user if found; null otherwise.</returns>
    public User GetUserByEmail(string email)
    {
        return _db.Table<User>().FirstOrDefault(u => u.Email == email);
    }

    /// <summary>
    /// Retrieves all students enrolled in classes taught by a specific teacher.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <returns>A distinct list of students across all of the teacher's classes.</returns>
    public List<User> GetStudentsByTeacherId(int teacherId)
    {
        return (from u in _db.Table<User>()
                join cs in _db.Table<ClassStudent>() on u.Id equals cs.StudentId
                join c in _db.Table<Class>() on cs.ClassId equals c.Id
                where c.TeacherId == teacherId
                select u).Distinct().ToList();
    }

    /// <summary>
    /// Retrieves a user by their authentication token.
    /// </summary>
    /// <param name="token">The authentication token to search for.</param>
    /// <returns>The matching user if found; null otherwise.</returns>
    public User GetUserByToken(string token)
    {
        return _db.Table<User>().FirstOrDefault(u => u.Token == token);
    }

    /// <summary>
    /// Retrieves all users with the student role.
    /// </summary>
    /// <returns>A list of all students in the system.</returns>
    public List<User> GetAllStudents()
    {
        return (from u in _db.Table<User>()
                where u.Role == "student"
                select u).ToList();
    }

    /// <summary>
    /// Retrieves all users with the teacher role.
    /// </summary>
    /// <returns>A list of all teachers in the system.</returns>
    public List<User> GetAllTeachers()
    {
        return (from u in _db.Table<User>()
                where u.Role == "teacher"
                select u).ToList();
    }

    /// <summary>
    /// Retrieves a student by their full name.
    /// </summary>
    /// <param name="name">The student's first name.</param>
    /// <param name="surname">The student's surname.</param>
    /// <returns>The matching student if found; null otherwise.</returns>
    public User GetStudentByName(string name, string surname)
    {
        return _db.Table<User>().FirstOrDefault(u =>
            u.Name == name &&
            u.Surname == surname &&
            u.Role == "student");
    }
}