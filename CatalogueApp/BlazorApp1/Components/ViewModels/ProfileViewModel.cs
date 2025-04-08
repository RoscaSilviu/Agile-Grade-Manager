using System.ComponentModel.DataAnnotations;
using CatalogueApp.Components.Services;

namespace CatalogueApp.ViewModels
{
    /// <summary>
    /// ViewModel for handling user profile operations in the Blazor client application.
    /// Manages the state and interactions for viewing profile information and changing passwords.
    /// </summary>
    /// <remarks>
    /// This ViewModel coordinates with the ProfileService to display user information
    /// and handle password change operations with validation.
    /// </remarks>
    public class ProfileViewModel
    {
        /// <summary>
        /// Gets or sets the user profile information.
        /// </summary>
        /// <remarks>
        /// Nullable to handle cases where profile data hasn't been loaded
        /// or when the user is not authenticated.
        /// </remarks>
        public UserResult? User { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether data is currently being loaded.
        /// </summary>
        /// <remarks>
        /// Initialized to true since data loading begins immediately.
        /// Used to manage loading states in the UI.
        /// </remarks>
        public bool IsLoading { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the change password form is visible.
        /// </summary>
        /// <remarks>
        /// Controls the visibility of the password change interface in the UI.
        /// </remarks>
        public bool IsChangePasswordVisible { get; set; } = false;

        /// <summary>
        /// Gets or sets the user's current password for verification.
        /// </summary>
        /// <remarks>
        /// Required for password change operations.
        /// Should never be persisted or logged.
        /// </remarks>
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the new password the user wishes to set.
        /// </summary>
        /// <remarks>
        /// Required for password change operations.
        /// Should never be persisted or logged.
        /// </remarks>
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the confirmation of the new password.
        /// </summary>
        /// <remarks>
        /// Must match the NewPassword value.
        /// Used for validation to prevent typing errors.
        /// </remarks>
        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error message for password change operations.
        /// </summary>
        /// <remarks>
        /// Will be null when no error has occurred.
        /// Contains user-friendly error messages when password change fails.
        /// </remarks>
        public string? ChangePasswordError { get; set; }

        /// <summary>
        /// Gets or sets the success message for password change operations.
        /// </summary>
        /// <remarks>
        /// Will be null when no operation has completed successfully.
        /// Contains confirmation message when password is changed.
        /// </remarks>
        public string? ChangePasswordSuccess { get; set; }

        /// <summary>
        /// Loads the user's profile information using the provided profile service.
        /// </summary>
        /// <param name="profileService">The service used to retrieve profile data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Updates the User property and loading state.
        /// Requires proper authentication via the profile service.
        /// </remarks>
        public async Task LoadProfileAsync(ProfileService profileService)
        {
            User = await profileService.GetUserProfileAsync();
            IsLoading = false;
        }

        /// <summary>
        /// Toggles the visibility of the change password form.
        /// </summary>
        /// <remarks>
        /// Resets all password-related fields and messages when toggled.
        /// Provides a clean state for new password change attempts.
        /// </remarks>
        public void ToggleChangePassword()
        {
            IsChangePasswordVisible = !IsChangePasswordVisible;
            ChangePasswordError = null;
            ChangePasswordSuccess = null;

            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

        /// <summary>
        /// Initiates the password change operation using the provided profile service.
        /// </summary>
        /// <param name="profileService">The service used to change the password.</param>
        /// <returns>
        /// True if the password was changed successfully;
        /// False if validation fails or the operation is unsuccessful.
        /// </returns>
        /// <remarks>
        /// Performs client-side validation before attempting the password change.
        /// Handles error messages and success states.
        /// Automatically hides the password form on success.
        /// </remarks>
        public async Task<bool> ChangePasswordAsync(ProfileService profileService)
        {
            ChangePasswordError = null;
            ChangePasswordSuccess = null;

            if (NewPassword != ConfirmPassword)
            {
                ChangePasswordError = "The new password and confirmation do not match.";
                return false;
            }

            bool result = await profileService.ChangePasswordAsync(CurrentPassword, NewPassword);
            if (result)
            {
                ChangePasswordSuccess = "Password changed successfully.";
                ToggleChangePassword();
                return true;
            }
            else
            {
                ChangePasswordError = "Failed to change password. Please check your current password and try again.";
                return false;
            }
        }
    }
}