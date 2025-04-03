using System.Net.Http.Json;
using Blazored.LocalStorage;
using CatalogueApp.Components.Models;

namespace CatalogueApp.Components.Services
{
    public class ClassService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        public ClassService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        public async Task<List<ClassModel>> GetClassesAsync(int teacherId)
        {
            // Retrieve the token from local storage.
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            // Add the token as a Bearer token to the request headers.
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Call the profile endpoint and deserialize the result.
            var result = await _httpClient.GetFromJsonAsync<List<ClassModel>>("api/Class");
            return result;
        }

        //public async Task<bool> AddClassAsync(string className)
        //{
        //    Ensure the auth token is set on the request headers
        //   var token = await _localStorage.GetItemAsync<string>("authToken");
        //    if (string.IsNullOrWhiteSpace(token))
        //        return false;
        //    _httpClient.DefaultRequestHeaders.Authorization =
        //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        //    var response = await _httpClient.PostAsJsonAsync("api/Class", classModel);
        //    return response.IsSuccessStatusCode;

        //}


    }
}
