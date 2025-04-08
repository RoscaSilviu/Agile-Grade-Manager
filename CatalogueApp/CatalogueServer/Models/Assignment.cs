using System;
using SQLite;

/// <summary>
/// Represents an academic assignment in the school management system.
/// </summary>
[Table("Assignments")]
public class Assignment
{
    /// <summary>
    /// Gets or sets the unique identifier for the assignment.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the assignment.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the detailed description of the assignment.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the due date for the assignment.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the ID of the class this assignment belongs to.
    /// </summary>
    [Indexed]
    public int ClassId { get; set; }

    /// <summary>
    /// Gets or sets the maximum possible grade for this assignment.
    /// </summary>
    public int MaxGrade { get; set; }

    /// <summary>
    /// Gets or sets the weight of this assignment in the final grade calculation.
    /// </summary>
    public int Weight { get; set; }
}