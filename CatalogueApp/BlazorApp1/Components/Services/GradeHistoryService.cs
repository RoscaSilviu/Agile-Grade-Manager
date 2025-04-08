using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace CatalogueApp.Components.Services
{
    /// <summary>
    /// Service responsible for managing grade history operations in the Blazor client application.
    /// Provides functionality to retrieve detailed grade history for specific subjects.
    /// </summary>
    /// <remarks>
    /// This service communicates with the server's GradeHistory endpoint and manages authentication
    /// using tokens stored in the browser's local storage.
    /// </remarks>
    public class GradeHistoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        /// <summary>
        /// Initializes a new instance of the GradeHistoryService.
        /// </summary>
        /// <param name="httpClient">HTTP client for making API requests.</param>
        /// <param name="localStorage">Local storage service for managing authentication tokens.</param>
        /// <exception cref="ArgumentNullException">Thrown when either parameter is null.</exception>
        public GradeHistoryService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        /// <summary>
        /// Retrieves the detailed grade history for a specific subject for the authenticated user.
        /// </summary>
        /// <param name="subject">The name of the subject to retrieve grades for.</param>
        /// <returns>
        /// A list of detailed grade history results if successful;
        /// null if the user is not authenticated or the request fails.
        /// </returns>
        /// <remarks>
        /// This method requires a valid authentication token in local storage.
        /// The subject name is automatically URL-encoded for safety.
        /// </remarks>
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

    /// <summary>
    /// Represents detailed grade history information for a single grade entry.
    /// </summary>
    public class GradeHistoryResult
    {
        /// <summary>
        /// Gets or sets the name of the subject.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the numerical grade value.
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// Gets or sets the date when the grade was assigned.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the name of the assignment.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string AssignmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the teacher who gave the grade.
        /// </summary>
        /// <remarks>
        /// Cannot be null, defaults to empty string if not specified.
        /// </remarks>
        public string TeacherName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets any additional comments about the grade.
        /// </summary>
        /// <remarks>
        /// Can be null if no comments were provided.
        /// </remarks>
        public string? Comments { get; set; }
    }
}