using System;
using SQLite;

[Table("Grades")]
public class Grade
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int StudentId { get; set; }

    [Indexed]
    public int AssignmentId { get; set; }
    public int Value { get; set; }
    public DateTime Date { get; set; }
}
