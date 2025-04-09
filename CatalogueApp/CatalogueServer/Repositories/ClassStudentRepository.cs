namespace CatalogueServer.Repositories
{
    public class ClassStudentRepository : BaseRepository<ClassStudent>
    {
        public ClassStudentRepository(Database database) : base(database.Connection) { }
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
    }
}
