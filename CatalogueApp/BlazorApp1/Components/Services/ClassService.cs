using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Blazored.LocalStorage;
using CatalogueApp.Components.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.JSInterop;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace CatalogueApp.Components.Services
{
    /// <summary>
    /// Service responsible for managing class-related operations in the Blazor client application.
    /// Handles class management, student enrollment, and assignment operations.
    /// </summary>
    /// <remarks>
    /// This service communicates with the server's Class endpoints and manages authentication
    /// using tokens stored in the browser's local storage.
    /// </remarks>
    public class ClassService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly IJSRuntime _jsRuntime;

        /// <summary>
        /// Initializes a new instance of the ClassService.
        /// </summary>
        /// <param name="httpClient">HTTP client for making API requests.</param>
        /// <param name="localStorage">Local storage service for managing authentication tokens.</param>
        /// <exception cref="ArgumentNullException">Thrown when either parameter is null.</exception>
        public ClassService(HttpClient httpClient, ILocalStorageService localStorage, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }


        /// <summary>
        /// Retrieves a full backup of the database.
        /// </summary>
        /// <returns>Complete database backup model.</returns>
        public async Task<string> GetFullBackupAsync()
        {
            // Instead of trying to deserialize as JSON
            var response = await _httpClient.GetAsync("api/Backup/text");
            response.EnsureSuccessStatusCode();

            await SaveToFile("FullBackup.txt", await response.Content.ReadAsStringAsync());

            return await response.Content.ReadAsStringAsync();
        }


public async Task SaveToFile(string fileName, string content)
    {
        // Create a base64 representation of the content
        var textBytes = Encoding.UTF8.GetBytes(content);
        var base64Text = Convert.ToBase64String(textBytes);

        // Create data URIs for text and CSV
        var txtDataUri = $"data:text/plain;base64,{base64Text}";
        var csvDataUri = $"data:text/csv;base64,{base64Text}";

        // Get file names without extension
        var fileNameWithoutExt = fileName.Contains(".")
            ? fileName.Substring(0, fileName.LastIndexOf('.'))
            : fileName;

        // Save as TXT
        await _jsRuntime.InvokeVoidAsync("eval", $@"
        (function() {{
            const link = document.createElement('a');
            link.href = '{txtDataUri}';
            link.download = '{fileNameWithoutExt}.txt';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }})();
    ");

        // Short delay to prevent browser blocking multiple downloads
        await Task.Delay(100);

        // Save as CSV
        await _jsRuntime.InvokeVoidAsync("eval", $@"
        (function() {{
            const link = document.createElement('a');
            link.href = '{csvDataUri}';
            link.download = '{fileNameWithoutExt}.csv';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }})();
    ");
    }

    /// <summary>
    /// Retrieves a backup of the current teacher's data.
    /// </summary>
    /// <returns>Teacher-specific backup model.</returns>
    public async Task<TeacherBackupModel> GetTeacherBackupAsync()
        {
            return await _httpClient.GetFromJsonAsync<TeacherBackupModel>("api/backup/teacher");
        }

        /// <summary>
        /// Retrieves a specific class by its name.
        /// </summary>
        /// <param name="className">Name of the class to retrieve.</param>
        /// <returns>The class model if found; null if unauthorized or not found.</returns>
        public async Task<ClassModel> GetClassByNameAsync(string className)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<ClassModel>($"api/Class/{className}");
        }

        /// <summary>
        /// Retrieves all classes for a specific teacher.
        /// </summary>
        /// <param name="teacherId">ID of the teacher.</param>
        /// <returns>List of classes or null if unauthorized.</returns>
        public async Task<List<ClassModel>> GetClassesAsync(int teacherId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<List<ClassModel>>("api/Class");
        }

        /// <summary>
        /// Retrieves all students in the system.
        /// </summary>
        /// <returns>List of all students.</returns>
        public async Task<List<StudentModel>> GetStudentsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<StudentModel>>("api/Class/GetStudents");
        }

        /// <summary>
        /// Creates a new class.
        /// </summary>
        /// <param name="className">Name of the class to create.</param>
        /// <returns>True if successful; false otherwise.</returns>
        public async Task<bool> AddClassAsync(string className)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("api/Class/AddClass", className);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Deletes a specific class.
        /// </summary>
        /// <param name="className">Name of the class to delete.</param>
        /// <returns>True if successful; false otherwise.</returns>
        public async Task<bool> DeleteClassAsync(string className)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"api/Class/DeleteClass?name={className}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Updates a class by deleting and recreating it.
        /// </summary>
        /// <param name="className">Name of the class to update.</param>
        /// <returns>True if successful; false if either operation fails.</returns>
        public async Task<bool> UpdateClassAsync(string className)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            var deleteResponse = await DeleteClassAsync(className);
            if (!deleteResponse)
                return false;
            return await AddClassAsync(className);
        }

        /// <summary>
        /// Adds a student to a specific class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="firstName">Student's first name.</param>
        /// <param name="lastName">Student's last name.</param>
        /// <returns>True if successful; false otherwise.</returns>
        public async Task<bool> AddStudentToClassAsync(string className, string firstName, string lastName)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var requestData = new StudentClassRequest
            {
                ClassName = className,
                FirstName = firstName,
                LastName = lastName
            };

            var response = await _httpClient.PostAsJsonAsync("api/Class/AddStudentToClass", requestData);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Retrieves all students enrolled in a specific class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns>List of students in the class.</returns>
        public async Task<List<StudentModel>> GetStudentsInClassAsync(string className)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Class/GetStudentsInClass/{className}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<StudentModel>>()
                        ?? new List<StudentModel>();
                }
                return new List<StudentModel>();
            }
            catch
            {
                return new List<StudentModel>();
            }
        }

        /// <summary>
        /// Retrieves all students not enrolled in a specific class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns>List of students not in the class, or null if unauthorized.</returns>
        public async Task<List<StudentModel>> GetAllStudentsNotInClassAsync(string className)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<List<StudentModel>>($"api/Class/GetAllStudentsNotInClass/{className}")
                ?? new List<StudentModel>();
        }

        /// <summary>
        /// Retrieves all students in the system.
        /// </summary>
        /// <returns>List of all students, or null if unauthorized.</returns>
        public async Task<List<StudentModel>> GetAllStudentsAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<List<StudentModel>>("api/Class/GetAllStudents")
                ?? new List<StudentModel>();
        }

        /// <summary>
        /// Removes a student from a specific class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="firstName">Student's first name.</param>
        /// <param name="lastName">Student's last name.</param>
        /// <returns>True if successful; false otherwise.</returns>
        public async Task<bool> RemoveStudentFromClassAsync(string className, string firstName, string lastName)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var encodedClassName = Uri.EscapeDataString(className);
            var encodedFirstName = Uri.EscapeDataString(firstName);
            var encodedLastName = Uri.EscapeDataString(lastName);

            var requestUrl = $"api/Class/RemoveStudentFromClass" +
                           $"?className={encodedClassName}" +
                           $"&firstName={encodedFirstName}" +
                           $"&lastName={encodedLastName}";
            var response = await _httpClient.DeleteAsync(requestUrl);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Retrieves all assignments for a specific class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns>List of assignments for the class.</returns>
        public async Task<List<AssignmentModel>> GetAssignmentsForClassAsync(string className)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token)) return new List<AssignmentModel>();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return await _httpClient.GetFromJsonAsync<List<AssignmentModel>>(
                $"api/Class/{className}/assignments") ?? new List<AssignmentModel>();
        }

        /// <summary>
        /// Adds a new assignment to a class.
        /// </summary>
        /// <param name="assignment">The assignment to add.</param>
        public async Task AddAssignmentToClassAsync(AssignmentModel assignment)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token)) return;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            await _httpClient.PostAsJsonAsync("api/Class/AddAssignment", assignment);
        }

        /// <summary>
        /// Updates an existing assignment.
        /// </summary>
        /// <param name="assignment">The updated assignment details.</param>
        public async Task UpdateAssignmentAsync(AssignmentModel assignment)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token)) return;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            await _httpClient.PutAsJsonAsync($"api/Class/UpdateAssignment/{assignment.Id}", assignment);
        }

        /// <summary>
        /// Deletes a specific assignment.
        /// </summary>
        /// <param name="assignmentId">ID of the assignment to delete.</param>
        public async Task DeleteAssignmentAsync(int assignmentId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token)) return;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            await _httpClient.DeleteAsync($"api/Class/DeleteAssignment/{assignmentId}");
        }
    }

    /// <summary>
    /// Represents a request to add or modify a student's class enrollment.
    /// </summary>
    public class StudentClassRequest
    {
        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the student's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the student's last name.
        /// </summary>
        public string LastName { get; set; }
    }
}