using System;
using SQLite;

[Table("Assignments")]
public class Assignment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }

    [Indexed]
    public int ClassId { get; set; }
    public int MaxGrade { get; set; }
    public int Weight { get; set; }
}