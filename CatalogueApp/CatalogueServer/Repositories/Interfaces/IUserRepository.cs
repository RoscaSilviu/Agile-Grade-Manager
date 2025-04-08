/// <summary>
/// Interface defining operations for managing users in the system.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The user if found; null otherwise.</returns>
    User GetUserByEmail(string email);

    /// <summary>
    /// Retrieves all students associated with a specific teacher.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <returns>A list of students in the teacher's classes.</returns>
    List<User> GetStudentsByTeacherId(int teacherId);

    /// <summary>
    /// Retrieves a user by their authentication token.
    /// </summary>
    /// <param name="token">The authentication token to search for.</param>
    /// <returns>The user if found; null otherwise.</returns>
    User GetUserByToken(string token);

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="user">The user to update.</param>
    void Update(User user);

    /// <summary>
    /// Inserts a new user into the system.
    /// </summary>
    /// <param name="entity">The user to insert.</param>
    void Insert(User entity);

    /// <summary>
    /// Deletes a user from the system.
    /// </summary>
    /// <param name="entity">The user to delete.</param>
    void Delete(User entity);

    /// <summary>
    /// Deletes all users from the system.
    /// </summary>
    void DeleteAll();

    /// <summary>
    /// Retrieves all users in the system.
    /// </summary>
    /// <returns>A list of all users.</returns>
    List<User> GetAll();

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The user if found; null otherwise.</returns>
    User GetById(int id);
}