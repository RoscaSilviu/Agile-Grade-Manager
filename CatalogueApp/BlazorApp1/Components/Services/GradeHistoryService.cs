using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace CatalogueApp.Components.Services
{
    public class GradeHistoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public GradeHistoryService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<List<GradeHistoryResult>?> GetGradeHistoryAsync(string subject)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await _httpClient.GetFromJsonAsync<List<GradeHistoryResult>>($"api/GradeHistory/{Uri.EscapeDataString(subject)}");
        }
    }

    public class GradeHistoryResult
    {
        public string Subject { get; set; } = string.Empty;
        public int Grade { get; set; }
        public DateTime Date { get; set; }
        public string AssignmentName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }
}