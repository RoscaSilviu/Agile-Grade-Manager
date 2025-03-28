using System.Linq;
using SQLite;


namespace CatalogueServer.Repositories
{

    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(SQLiteConnection db) : base(db) { }

        // Get user by email
        public User GetUserByEmail(string email)
        {
            return _db.Table<User>().FirstOrDefault(u => u.Email == email);
        }

        // Get all students in a teacher's classes
        public List<User> GetStudentsByTeacherId(int teacherId)
        {
            return (from u in _db.Table<User>()
                    join cs in _db.Table<ClassStudent>() on u.Id equals cs.StudentId
                    join c in _db.Table<Class>() on cs.ClassId equals c.Id
                    where c.TeacherId == teacherId
                    select u).Distinct().ToList();
        }


    }
}
