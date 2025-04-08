using Blazored.LocalStorage;
using CatalogueApp.Components.Models;

namespace CatalogueApp.Components.Services
{
    /// <summary>
    /// Service responsible for managing assignment-related operations in the Blazor client application.
    /// Provides methods for retrieving and saving grade information for assignments.
    /// </summary>
    /// <remarks>
    /// This service handles communication with the server's grade endpoints and manages local storage.
    /// </remarks>
    public class AssignmentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        /// <summary>
        /// Initializes a new instance of the AssignmentService.
        /// </summary>
        /// <param name="httpClient">HTTP client for making API requests.</param>
        /// <param name="localStorage">Local storage service for caching data.</param>
        /// <exception cref="ArgumentNullException">Thrown when either parameter is null.</exception>
        public AssignmentService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        /// <summary>
        /// Retrieves grades for a specific assignment from the server.
        /// </summary>
        /// <param name="assignmentId">The ID of the assignment to get grades for.</param>
        /// <returns>A list of student grades for the specified assignment.</returns>
        /// <exception cref="Exception">Thrown when the server request fails.</exception>
        /// <remarks>
        /// Makes an HTTP GET request to the grades API endpoint.
        /// The response includes student information along with their grades.
        /// </remarks>
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

        /// <summary>
        /// Saves grades for a specific assignment to the server.
        /// </summary>
        /// <param name="assignmentId">The ID of the assignment these grades belong to.</param>
        /// <param name="grades">The list of student grades to save.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the server request fails.</exception>
        /// <remarks>
        /// Makes an HTTP POST request to the grades API endpoint.
        /// All grades are saved in a single transaction.
        /// </remarks>
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