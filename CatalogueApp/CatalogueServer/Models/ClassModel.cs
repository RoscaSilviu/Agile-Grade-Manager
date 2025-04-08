using System;
using SQLite;

/// <summary>
/// Represents a class or course in the school management system.
/// </summary>
[Table("Classes")]
public class Class
{
    /// <summary>
    /// Gets or sets the unique identifier for the class.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the class.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the ID of the teacher responsible for this class.
    /// </summary>
    [Indexed]
    public int TeacherId { get; set; }
}