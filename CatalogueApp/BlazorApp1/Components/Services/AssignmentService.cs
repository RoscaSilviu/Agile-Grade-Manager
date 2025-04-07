using Blazored.LocalStorage;
using CatalogueApp.Components.Models;

namespace CatalogueApp.Components.Services
{
    public class AssignmentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;


        public AssignmentService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        //GetGradesForAssignmentAsync
        public async Task<List<StudentGradeModel>> GetGradesForAssignmentAsync(int assignmentId)
        {
            var response = await _httpClient.GetAsync($"api/grades/assignment/{assignmentId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<StudentGradeModel>>();
            }
            else
            {
                throw new Exception("Failed to fetch grades");
            }
        }

        //SaveGradesAsync
        public async Task SaveGradesAsync(int assignmentId, List<StudentGradeModel> grades)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/grades/assignment/{assignmentId}", grades);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to save grades");
            }
        }
    }
}
