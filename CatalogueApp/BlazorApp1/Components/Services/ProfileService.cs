using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace CatalogueApp.Components.Services
{
    /// <summary>
    /// Service responsible for managing user profile operations in the Blazor client application.
    /// Provides functionality to retrieve user profile information and manage password changes.
    /// </summary>
    /// <remarks>
    /// This service communicates with the server's Profile endpoint and manages authentication
    /// using tokens stored in the browser's local storage.
    /// </remarks>
    public class ProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        /// <summary>
        /// Initializes a new instance of the ProfileService.
        /// </summary>
        /// <param name="httpClient">HTTP client for making API requests.</param>
        /// <param name="localStorage">Local storage service for managing authentication tokens.</param>
        /// <exception cref="ArgumentNullException">Thrown when either parameter is null.</exception>
        public ProfileService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        /// <summary>
        /// Retrieves the profile information for the authenticated user.
        /// </summary>
        /// <returns>
        /// The user's profile information if successful;
        /// null if the user is not authenticated or the request fails.
        /// </returns>
        /// <remarks>
        /// This method requires a valid authentication token in local storage.
        /// The token is automatically included in the request header.
        /// </remarks>
        public async Task<UserResult?> GetUserProfileAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var result = await _httpClient.GetFromJsonAsync<UserResult>("api/Profile");
            return result;
        }

        /// <summary>
        /// Changes the password for the authenticated user.
        /// </summary>
        /// <param name="currentPassword">The user's current password.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>
        /// True if the password was changed successfully;
        /// False if the user is not authenticated or the change fails.
        /// </returns>
        /// <remarks>
        /// This method requires a valid authentication token in local storage.
        /// Passwords are transmitted securely but should never be logged.
        /// </remarks>
        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
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

    /// <summary>
    /// Represents user profile information returned by the API.
    /// </summary>
    /// <remarks>
    /// Contains only non-sensitive user information suitable for client display.
    /// All string properties are initialized to empty strings to prevent null values.
    /// </remarks>
    public class UserResult
    {
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's surname.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string Surname { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// Used as the username for authentication.
        /// </remarks>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's role in the system.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// Common values are "teacher" and "student".
        /// </remarks>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp of the user's last login.
        /// </summary>
        public DateTime LastLogin { get; set; }
    }
}