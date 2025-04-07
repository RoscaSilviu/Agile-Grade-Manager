using System.Linq;
using SQLite;

namespace CatalogueServer.Repositories
{
    public class ClassRepository : BaseRepository<Class>
    {
        public ClassRepository(Database database) : base(database.Connection) { }

        // Get all classes for a specific teacher
        public List<Class> GetClassesByTeacherId(int teacherId)
        {
            return _db.Table<Class>().Where(c => c.TeacherId == teacherId).ToList();
        }

        public void AddStudentToClass(int classId, int studentId)
        {
            var classStudent = new ClassStudent { ClassId = classId, StudentId = studentId };
            _db.Insert(classStudent);
        }

        // Remove a student from a class
        public void RemoveStudentFromClass(int classId, int studentId)
        {
            var classStudent = _db.Table<ClassStudent>()
                                  .FirstOrDefault(cs => cs.ClassId == classId && cs.StudentId == studentId);
            if (classStudent != null)
            {
                _db.Delete(classStudent);
            }
        }

        // Get all students in a class
        public List<User> GetStudentsInClass(int classId)
        {
            var studentIds = _db.Table<ClassStudent>()
                                .Where(cs => cs.ClassId == classId)
                                .Select(cs => cs.StudentId)
                                .ToList();

            return _db.Table<User>().Where(u => studentIds.Contains(u.Id)).ToList();
        }

        // Get all classes for a specific student
        public List<Class> GetClassesByStudentId(int studentId)
        {
            return (from c in _db.Table<Class>()
                    join cs in _db.Table<ClassStudent>() on c.Id equals cs.ClassId
                    where cs.StudentId == studentId
                    select c).ToList();
        }


        // Get a class by its name and teacher ID
        public Class GetClassByNameAndTeacherId(string name, int teacherId)
        {
            return _db.Table<Class>().FirstOrDefault(c => c.Name == name && c.TeacherId == teacherId);
        }
    }
}
