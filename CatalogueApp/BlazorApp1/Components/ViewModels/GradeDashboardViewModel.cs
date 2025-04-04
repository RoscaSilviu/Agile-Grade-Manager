using System.ComponentModel.DataAnnotations;
using CatalogueApp.Components.Services;
using Microsoft.AspNetCore.Components;

namespace CatalogueApp.Components.ViewModels
{
    public class GradeDashboardViewModel
    {
        public List<GradeAverageResult> Grades { get; set; } = new();
        public bool IsLoading { get; set; } = true;
        public string? ErrorMessage { get; set; }

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