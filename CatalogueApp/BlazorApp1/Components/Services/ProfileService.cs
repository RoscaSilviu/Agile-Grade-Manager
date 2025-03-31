using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace CatalogueApp.Components.Services
{
    public class ProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ProfileService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<UserResult?> GetUserProfileAsync()
        {
            // Retrieve the token from local storage.
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            // Add the token as a Bearer token to the request headers.
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Call the profile endpoint and deserialize the result.
            var result = await _httpClient.GetFromJsonAsync<UserResult>("api/Profile");
            return result;
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            // Ensure the auth token is set on the request headers
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var requestData = new
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            var response = await _httpClient.PostAsJsonAsync("api/Profile/ChangePassword", requestData);
            return response.IsSuccessStatusCode;
        }
    }
    public class UserResult
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime LastLogin { get; set; }
    }
}
