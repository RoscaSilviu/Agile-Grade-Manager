namespace CatalogueApp.Components.Models
{
    /// <summary>
    /// Represents a student's grade information in the Blazor client application.
    /// Used for displaying and managing individual student grades for assignments.
    /// </summary>
    /// <remarks>
    /// This model connects student information with their grade data.
    /// Commonly used in grade lists and assignment grading interfaces.
    /// </remarks>
    public class StudentGradeModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the student.
        /// </summary>
        /// <remarks>
        /// Used to link the grade to a specific student in the system.
        /// References the student's primary key in the database.
        /// </remarks>
        public int StudentId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the student.
        /// </summary>
        /// <remarks>
        /// Typically contains the student's first and last name.
        /// Used for display purposes in grading interfaces and reports.
        /// </remarks>
        public string StudentName { get; set; }

        /// <summary>
        /// Gets or sets the numerical grade value assigned to the student.
        /// </summary>
        /// <remarks>
        /// Represents the points earned by the student for an assignment.
        /// Should be within the valid range defined by the assignment's maximum points.
        /// </remarks>
        public int Grade { get; set; }
    }
}