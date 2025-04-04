using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace CatalogueApp.Components.Services
{
    public class GradeDashboardService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public GradeDashboardService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<List<GradeAverageResult>?> GetGradesAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await _httpClient.GetFromJsonAsync<List<GradeAverageResult>>("api/GradeDashboard");
        }
    }

    public class GradeAverageResult
    {
        public string Subject { get; set; } = string.Empty;
        public double AverageGrade { get; set; }
        public DateTime LastGraded { get; set; }
    }
}