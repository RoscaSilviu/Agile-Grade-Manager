namespace CatalogueApp.Components.Models
{
    /// <summary>
    /// Represents an academic class in the Blazor client application.
    /// Used for managing class information and student enrollments.
    /// </summary>
    /// <remarks>
    /// This model serves as the primary container for class-related data,
    /// maintaining the relationship between classes and enrolled students.
    /// Used in both display and management operations throughout the application.
    /// </remarks>
    public class ClassModel
    {
        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <remarks>
        /// Serves as the primary identifier for the class in the UI.
        /// Used for displaying, sorting, and organizing classes.
        /// Should be unique within a teacher's class list.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of students enrolled in the class.
        /// </summary>
        /// <remarks>
        /// Contains the complete list of students currently enrolled in this class.
        /// Used for managing class rosters and displaying student information.
        /// Should be initialized to prevent null reference exceptions.
        /// </remarks>
        public List<StudentModel> Students { get; set; }
    }
}