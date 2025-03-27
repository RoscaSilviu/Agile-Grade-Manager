using System;
using SQLite;

[Table("Classes")]
public class Class
{
    [PrimaryKey, AutoIncrement]

    public int Id { get; set; }
    public string Name { get; set; }

    [Indexed]
    public int TeacherId { get; set; }

}