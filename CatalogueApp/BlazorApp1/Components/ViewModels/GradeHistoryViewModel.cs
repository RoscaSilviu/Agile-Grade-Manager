using CatalogueApp.Components.Services;
using Microsoft.AspNetCore.Components;

namespace CatalogueApp.Components.ViewModels
{
    public class GradeHistoryViewModel
    {
        public List<GradeHistoryResult> Grades { get; set; } = new();
        public bool IsLoading { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public string Subject { get; set; } = string.Empty;

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