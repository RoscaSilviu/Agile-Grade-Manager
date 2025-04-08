using SQLite;

/// <summary>
/// Represents a many-to-many relationship between classes and students.
/// This model associates students with their enrolled classes.
/// </summary>
[Table("ClassStudents")]
public class ClassStudent
{
    /// <summary>
    /// Gets or sets the unique identifier for the class-student relationship.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the class.
    /// </summary>
    [Indexed]
    public int ClassId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the student enrolled in the class.
    /// </summary>
    [Indexed]
    public int StudentId { get; set; }
}