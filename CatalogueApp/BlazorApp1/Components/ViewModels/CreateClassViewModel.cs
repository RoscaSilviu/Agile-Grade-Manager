using System.ComponentModel.DataAnnotations;
using CatalogueApp.Components.Services;
using System.Threading.Tasks;

namespace CatalogueApp.ViewModels
{
    public class CreateClassViewModel
    {
        [Required(ErrorMessage = "Class name is required")]
        [StringLength(50, ErrorMessage = "Class name cannot exceed 50 characters")]
        public string ClassName { get; set; }

        public bool IsLoading { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task CreateClassAsync(ClassService classService)
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            try
            {
                // Replace with actual service call
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