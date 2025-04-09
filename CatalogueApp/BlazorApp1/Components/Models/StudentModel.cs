namespace CatalogueApp.Components.Models
{
    /// <summary>
    /// Represents a student's basic information in the Blazor client application.
    /// Used for displaying and managing student data across components.
    /// </summary>
    /// <remarks>
    /// This model contains core student identification information.
    /// Used in student lists, class rosters, and enrollment interfaces.
    /// </remarks>
    public class StudentModel
    {
        /// <summary>
        /// Gets or sets the student's first name.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// Used for display and sorting in student lists.
        /// </remarks>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the student's surname (family name).
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// Used for display and alphabetical sorting in student lists.
        /// </remarks>
        public string Surname { get; set; } = string.Empty;

        public List<GradeModel> Grades { get; set; } = new();
    }
}