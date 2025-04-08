using System.ComponentModel.DataAnnotations;
using CatalogueApp.Components.Services;
using System.Threading.Tasks;

namespace CatalogueApp.ViewModels
{
    /// <summary>
    /// ViewModel for handling class creation operations in the Blazor client application.
    /// Manages the form state and interaction with the class service.
    /// </summary>
    /// <remarks>
    /// This ViewModel supports data validation and provides feedback during the class creation process.
    /// </remarks>
    public class CreateClassViewModel
    {
        /// <summary>
        /// Gets or sets the name of the class to be created.
        /// </summary>
        /// <remarks>
        /// The class name must not be empty and cannot exceed 50 characters.
        /// This property is required for form submission.
        /// </remarks>
        [Required(ErrorMessage = "Class name is required")]
        [StringLength(50, ErrorMessage = "Class name cannot exceed 50 characters")]
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an operation is in progress.
        /// </summary>
        /// <remarks>
        /// Used to manage loading states and disable UI elements during operations.
        /// </remarks>
        public bool IsLoading { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the last operation was successful.
        /// </summary>
        /// <remarks>
        /// Used to determine what feedback to show to the user.
        /// </remarks>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error message to display when an operation fails.
        /// </summary>
        /// <remarks>
        /// Will be null when no error has occurred.
        /// </remarks>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the success message to display after a successful operation.
        /// </summary>
        /// <remarks>
        /// Will be null when no operation has completed successfully.
        /// </remarks>
        public string SuccessMessage { get; set; }

        /// <summary>
        /// Creates a new class using the provided class service.
        /// </summary>
        /// <param name="classService">The service used to create the class.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method handles the complete creation workflow including:
        /// - Setting loading states
        /// - Error handling
        /// - Success messaging
        /// The operation requires proper authentication via the class service.
        /// </remarks>
        public async Task CreateClassAsync(ClassService classService)
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            try
            {
                await classService.AddClassAsync(ClassName);

                Success = true;
                SuccessMessage = "Class created successfully! Redirecting...";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Success = false;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}