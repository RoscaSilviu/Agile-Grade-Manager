using System.ComponentModel.DataAnnotations;
using CatalogueApp.Components.Services;

namespace CatalogueApp.ViewModels
{
    public class ProfileViewModel
    {
        public UserResult? User { get; set; }
        public bool IsLoading { get; set; } = true;

        public bool IsChangePasswordVisible { get; set; } = false;

        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ChangePasswordError { get; set; }
        public string? ChangePasswordSuccess { get; set; }


        public async Task LoadProfileAsync(ProfileService profileService)
        {
            User = await profileService.GetUserProfileAsync();
            IsLoading = false;
        }


        public void ToggleChangePassword()
        {
            IsChangePasswordVisible = !IsChangePasswordVisible;
            ChangePasswordError = null;
            ChangePasswordSuccess = null;

            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

    
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
