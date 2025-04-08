using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace CatalogueApp.Components.Services
{
    /// <summary>
    /// Service responsible for handling authentication operations in the Blazor client application.
    /// Manages user login and token storage.
    /// </summary>
    /// <remarks>
    /// This service communicates with the server's Auth endpoint and manages authentication tokens
    /// in the browser's local storage.
    /// </remarks>
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        /// <summary>
        /// Initializes a new instance of the AuthService.
        /// </summary>
        /// <param name="httpClient">HTTP client for making API requests.</param>
        /// <param name="localStorage">Local storage service for storing authentication tokens.</param>
        /// <exception cref="ArgumentNullException">Thrown when either parameter is null.</exception>
        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        /// <summary>
        /// Authenticates a user with their email and password.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>
        /// A tuple containing the authentication token and user role if successful;
        /// null values if authentication fails.
        /// </returns>
        /// <remarks>
        /// On successful login, stores the authentication token in local storage
        /// for subsequent API requests.
        /// </remarks>
        public async Task<(string?, string?)> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/login",
                new LoginRequest { Email = email, Password = password }
            );

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                await _localStorage.SetItemAsync("authToken", result!.Token);
                return (result.Token, result.Role);
            }
            return (null, null);
        }
    }

    /// <summary>
    /// Represents a login request to the authentication API.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the email address for authentication.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for authentication.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// The password is transmitted securely but should never be logged.
        /// </remarks>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the response from a successful login attempt.
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// Gets or sets the authentication token for the user session.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// This token should be included in subsequent API requests.
        /// </remarks>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the role of the authenticated user.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// Used for client-side authorization and UI customization.
        /// </remarks>
        public string Role { get; set; } = string.Empty;
    }
}