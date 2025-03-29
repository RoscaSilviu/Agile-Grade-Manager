using System.Collections.Generic;
using System.Linq;
using SQLite;


namespace CatalogueServer.Repositories
{
    public class AssignmentRepository : BaseRepository<Assignment>
    {
        public AssignmentRepository(Database database) : base(database.Connection) { }


        // Get all assignments for a class
        public List<Assignment> GetAssignmentsByClassId(int classId)
        {
            return _db.Table<Assignment>().Where(a => a.ClassId == classId).ToList();
        }

        // Get all assignments for a teacher (based on their classes)
        public List<Assignment> GetAssignmentsByTeacherId(int teacherId)
        {
            var teacherClasses = _db.Table<Class>().Where(c => c.TeacherId == teacherId).Select(c => c.Id).ToList();
            return _db.Table<Assignment>().Where(a => teacherClasses.Contains(a.ClassId)).ToList();
        }
    }
}
