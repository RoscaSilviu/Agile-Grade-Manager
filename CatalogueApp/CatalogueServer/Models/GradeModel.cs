using System;
using SQLite;

/// <summary>
/// Represents a grade assigned to a student for a specific assignment.
/// </summary>
[Table("Grades")]
public class Grade
{
    /// <summary>
    /// Gets or sets the unique identifier for the grade.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the student who received this grade.
    /// </summary>
    [Indexed]
    public int StudentId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the assignment this grade is for.
    /// </summary>
    [Indexed]
    public int AssignmentId { get; set; }

    /// <summary>
    /// Gets or sets the numerical value of the grade.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets the date when the grade was assigned.
    /// </summary>
    public DateTime Date { get; set; }
}