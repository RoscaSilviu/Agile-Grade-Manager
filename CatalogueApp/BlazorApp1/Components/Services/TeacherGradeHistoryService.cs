// BlazorApp1/Components/Services/TeacherGradeHistoryService.cs
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using CatalogueServer.Controllers;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp1.Components.Services
{
    public class TeacherGradeHistoryService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public TeacherGradeHistoryService(
            HttpClient http,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        private async Task<int?> GetTeacherId()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var user = await _http.GetFromJsonAsync<UserResult>("api/Profile");

            // Check if user has teacher role
            if (user.Role != "teacher")
            {
                throw new UnauthorizedAccessException("User is not a teacher.");
            }

            return user.Id;
        }

        public async Task<List<TeacherGradeDetail>> GetTeacherGradesAsync()
        {
            var teacherId = await GetTeacherId();
            if (teacherId == null)
                return new List<TeacherGradeDetail>();

            return await _http.GetFromJsonAsync<List<TeacherGradeDetail>>(
                $"api/gradehistory/teacher/{teacherId}") ?? new();
        }

        public async Task UpdateGradeAsync(int gradeId, int newValue)
        {
            await _http.PutAsJsonAsync(
                $"api/gradehistory/teacher/update/{gradeId}", newValue);
        }

        public async Task DeleteGradeAsync(int gradeId)
        {
            await _http.DeleteAsync($"api/gradehistory/teacher/{gradeId}");
        }
    }
}