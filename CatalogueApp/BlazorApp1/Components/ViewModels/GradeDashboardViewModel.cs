using System.ComponentModel.DataAnnotations;
using CatalogueApp.Components.Services;
using Microsoft.AspNetCore.Components;

namespace CatalogueApp.Components.ViewModels
{
    /// <summary>
    /// ViewModel for handling grade dashboard operations in the Blazor client application.
    /// Manages the state and data loading for displaying student grade averages.
    /// </summary>
    /// <remarks>
    /// This ViewModel coordinates with the GradeDashboardService to fetch and display grade data,
    /// handling authentication redirects and error states.
    /// </remarks>
    public class GradeDashboardViewModel
    {
        /// <summary>
        /// Gets or sets the collection of grade averages for all subjects.
        /// </summary>
        /// <remarks>
        /// Initialized as an empty list to prevent null reference exceptions.
        /// Updated when grades are successfully loaded from the service.
        /// </remarks>
        public List<GradeAverageResult> Grades { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether data is currently being loaded.
        /// </summary>
        /// <remarks>
        /// Initialized to true since data loading begins immediately.
        /// Used to manage loading states in the UI.
        /// </remarks>
        public bool IsLoading { get; set; } = true;

        /// <summary>
        /// Gets or sets the error message when grade loading fails.
        /// </summary>
        /// <remarks>
        /// Will be null when no error has occurred.
        /// Contains user-friendly error messages when exceptions occur.
        /// </remarks>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Loads grade data using the provided grade service.
        /// </summary>
        /// <param name="gradeService">The service used to retrieve grade data.</param>
        /// <param name="navigation">Navigation manager for handling authentication redirects.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method handles the complete grade loading workflow including:
        /// - Authentication checking
        /// - Error handling
        /// - Navigation redirection for unauthenticated users
        /// - State management during loading
        /// </remarks>
        public async Task LoadGradesAsync(GradeDashboardService gradeService, NavigationManager navigation)
        {
            try
            {
                var grades = await gradeService.GetGradesAsync();
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