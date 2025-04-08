namespace CatalogueApp.Components.Models
{
    /// <summary>
    /// Represents a backup model for teacher-specific data in the Blazor client application.
    /// Contains a teacher's personal information and their associated classes.
    /// </summary>
    /// <remarks>
    /// Used for exporting and storing teacher data including their class assignments
    /// and student enrollments.
    /// </remarks>
    public class TeacherBackupModel
    {
        /// <summary>
        /// Gets or sets the name of the teacher.
        /// </summary>
        /// <remarks>
        /// Used to identify the teacher whose data is being backed up.
        /// Typically includes the full name of the teacher.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of classes taught by the teacher.
        /// </summary>
        /// <remarks>
        /// Contains all class information including enrolled students.
        /// Used for complete restoration of a teacher's academic data.
        /// </remarks>
        public List<ClassModel> Classes { get; set; }
    }

    /// <summary>
    /// Represents a complete system backup model in the Blazor client application.
    /// Contains all system data including teachers, classes, students, and assignments.
    /// </summary>
    /// <remarks>
    /// Used for full system backups and data migration.
    /// Implements a comprehensive backup strategy for the entire application.
    /// </remarks>
    public class FullBackupModel
    {
        // Implementation to be added based on system requirements
    }
}