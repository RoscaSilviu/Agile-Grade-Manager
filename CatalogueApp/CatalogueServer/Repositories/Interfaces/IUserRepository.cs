public interface IUserRepository
{
    User GetUserByEmail(string email);
    List<User> GetStudentsByTeacherId(int teacherId);
    User GetUserByToken(string token);
    void Update(User user);
    void Insert(User entity);
    void Delete(User entity);
    void DeleteAll();
    List<User> GetAll();
    User GetById(int id);
}