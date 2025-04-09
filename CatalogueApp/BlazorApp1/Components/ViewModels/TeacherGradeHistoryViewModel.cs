using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp1.Components.Services;
using CatalogueServer.Controllers;
using CatalogueServer.Repositories;
using Microsoft.AspNetCore.Components;
using static CatalogueServer.Repositories.GradeRepository;

namespace BlazorApp1.Components.ViewModels
{
    public class TeacherGradeHistoryViewModel
    {
        public List<TeacherGradeDetail> Grades { get; private set; } = new();
        public bool IsLoading { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int? EditingGradeId { get; private set; }
        public int EditValue { get; set; }

        public async Task LoadGradesAsync(TeacherGradeHistoryService service, NavigationManager navigation)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                Grades = await service.GetTeacherGradesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load grades. Please try again later.";
                navigation.NavigateTo("/profile");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void StartEdit(TeacherGradeDetail grade)
        {
            EditingGradeId = grade.GradeId;
            EditValue = grade.Value;
        }

        public void CancelEdit()
        {
            EditingGradeId = null;
        }

        public async Task SaveEditAsync(TeacherGradeHistoryService service)
        {
            if (EditingGradeId.HasValue)
            {
                try
                {
                    await service.UpdateGradeAsync(EditingGradeId.Value, EditValue);
                    EditingGradeId = null;
                    await LoadGradesAsync(service, null!);
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Failed to update grade. Please try again.";
                }
            }
        }

        public async Task DeleteGradeAsync(TeacherGradeHistoryService service, int gradeId)
        {
            try
            {
                await service.DeleteGradeAsync(gradeId);
                await LoadGradesAsync(service, null!);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to delete grade. Please try again.";
            }
        }

        public async Task UploadGradesAsync(TeacherGradeHistoryService service, List<GradeUploadModel> grades)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                await service.UploadGradesAsync(grades);
                await LoadGradesAsync(service, null!);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to upload grades. Please verify the CSV format and try again.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}