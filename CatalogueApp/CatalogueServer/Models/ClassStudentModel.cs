using SQLite;


[Table("ClassStudents")]
public class ClassStudent
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int ClassId { get; set; }

    [Indexed]
    public int StudentId { get; set; }

}
