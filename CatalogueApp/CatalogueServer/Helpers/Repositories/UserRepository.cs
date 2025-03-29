using System.Linq;
using SQLite;
using CatalogueServer.Helpers;


namespace CatalogueServer.Helpers.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(Database database)
                : base(database.GetConnection()) { }
        public void AddUser(string email, string password, string role, string name, string surname, string token)
        {
            var user = new User
            {
                Email = email,
                Password = PasswordHelper.HashPassword(password),
                Role = role,
                Name = name,
                Surname = surname,
                Token = token,
                LastLogin = DateTime.Now
            };
            Insert(user);
        }

        public User GetUserByEmail(string email)
        {
            return _db.Table<User>().FirstOrDefault(u => u.Email == email);
        }

        public void UpdateUser(User user)
        {
            Update(user);
        }

        public void DeleteUser(User user)
        {
            Delete(user);
        }

        public void DeleteAllUsers()
        {
            DeleteAll();
        }

        public List<User> GetAllUsers()
        {
            return GetAll();
        }

        //Get all students
        public List<User> GetStudents()
        {
            return _db.Table<User>().Where(u => u.Role == "Student").ToList();
        }

        //Get all teachers
        public List<User> GetTeachers()
        {
            return _db.Table<User>().Where(u => u.Role == "Teacher").ToList();
        }

        //GetStudentByEmail
        public User GetStudentByEmail(string email)
        {
            return _db.Table<User>().FirstOrDefault(u => u.Email == email && u.Role == "Student");
        }
    }
}
