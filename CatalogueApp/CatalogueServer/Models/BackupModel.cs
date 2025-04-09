namespace CatalogueServer.Models
{
    public class FullBackupModel
    {
        public List<Class> Classes { get; set; }
        public List<User> Students { get; set; }
        public List<Assignment> Assignments { get; set; }

        public List<User> Teachers { get; set; }
        public List<Grade> Grades { get; set; }
    }

}
