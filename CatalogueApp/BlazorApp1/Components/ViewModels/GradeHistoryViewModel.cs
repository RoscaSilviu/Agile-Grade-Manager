using CatalogueApp.Components.Services;
using Microsoft.AspNetCore.Components;

namespace CatalogueApp.Components.ViewModels
{
    /// <summary>
    /// ViewModel for handling grade history operations in the Blazor client application.
    /// Manages the state and data loading for displaying detailed grade history for a specific subject.
    /// </summary>
    /// <remarks>
    /// This ViewModel coordinates with the GradeHistoryService to fetch and display detailed grade data,
    /// handling authentication redirects and error states.
    /// </remarks>
    public class GradeHistoryViewModel
    {
        /// <summary>
        /// Gets or sets the collection of detailed grade history entries.
        /// </summary>
        /// <remarks>
        /// Initialized as an empty list to prevent null reference exceptions.
        /// Updated when grade history is successfully loaded from the service.
        /// </remarks>
        public List<GradeHistoryResult> Grades { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether data is currently being loaded.
        /// </summary>
        /// <remarks>
        /// Initialized to true since data loading begins immediately.
        /// Used to manage loading states in the UI.
        /// </remarks>
        public bool IsLoading { get; set; } = true;

        /// <summary>
        /// Gets or sets the error message when grade history loading fails.
        /// </summary>
        /// <remarks>
        /// Will be null when no error has occurred.
        /// Contains user-friendly error messages when exceptions occur.
        /// </remarks>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the name of the subject being viewed.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// Updated when loading grade history for a specific subject.
        /// </remarks>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Loads grade history data for a specific subject using the provided grade service.
        /// </summary>
        /// <param name="gradeService">The service used to retrieve grade history data.</param>
        /// <param name="subject">The name of the subject to load grades for.</param>
        /// <param name="navigation">Navigation manager for handling authentication redirects.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method handles the complete grade history loading workflow including:
        /// - Authentication checking
        /// - Error handling
        /// - Navigation redirection for unauthenticated users
        /// - State management during loading
        /// The subject parameter is automatically URL-encoded by the service.
        /// </remarks>
        public async Task LoadGradeHistoryAsync(GradeHistoryService gradeService, string subject, NavigationManager navigation)
        {
            try
            {
                Subject = subject;
                var grades = await gradeService.GetGradeHistoryAsync(subject);
                if (grades == null)
                {
                    navigation.NavigateTo("/login");
                    return;
                }

                Grades = grades;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                Grades.Clear();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}