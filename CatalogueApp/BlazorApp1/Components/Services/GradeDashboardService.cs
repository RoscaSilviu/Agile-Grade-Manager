using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace CatalogueApp.Components.Services
{
    /// <summary>
    /// Service responsible for managing grade dashboard operations in the Blazor client application.
    /// Provides functionality to retrieve grade averages for the authenticated user.
    /// </summary>
    /// <remarks>
    /// This service communicates with the server's GradeDashboard endpoint and manages authentication
    /// using tokens stored in the browser's local storage.
    /// </remarks>
    public class GradeDashboardService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        /// <summary>
        /// Initializes a new instance of the GradeDashboardService.
        /// </summary>
        /// <param name="httpClient">HTTP client for making API requests.</param>
        /// <param name="localStorage">Local storage service for managing authentication tokens.</param>
        /// <exception cref="ArgumentNullException">Thrown when either parameter is null.</exception>
        public GradeDashboardService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        /// <summary>
        /// Retrieves the average grades for all subjects for the authenticated user.
        /// </summary>
        /// <returns>
        /// A list of grade averages per subject if successful;
        /// null if the user is not authenticated or the request fails.
        /// </returns>
        /// <remarks>
        /// This method requires a valid authentication token in local storage.
        /// The token is automatically included in the request header.
        /// </remarks>
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

    /// <summary>
    /// Represents the average grade information for a subject.
    /// </summary>
    public class GradeAverageResult
    {
        /// <summary>
        /// Gets or sets the name of the subject.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the calculated average grade for the subject.
        /// </summary>
        /// <remarks>
        /// Represents the weighted average of all assignments in the subject.
        /// </remarks>
        public double AverageGrade { get; set; }

        /// <summary>
        /// Gets or sets the date of the most recent grade in this subject.
        /// </summary>
        public DateTime LastGraded { get; set; }
    }
}