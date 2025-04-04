﻿using System.Net.Http.Json;
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

        public async Task<List<StudentModel>> GetStudentsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<StudentModel>>($"api/Class/GetStudents");
            return result;
        }

        public async Task<bool> AddClassAsync(string className)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var requestData = new
            {
                Name = className
            };

            var response = await _httpClient.PostAsJsonAsync("api/Class/AddClass", className);
            return response.IsSuccessStatusCode;

        }

        public async Task<bool> DeleteClassAsync(string className)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            //delete the clasds with the name classNameand the teacherId associated with the token

            var response = await _httpClient.DeleteAsync($"api/Class/DeleteClass?name={className}");


            return response.IsSuccessStatusCode;
        }

        //update class method should call delete class method and add class method
        public async Task<bool> UpdateClassAsync(string className)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var deleteResponse = await DeleteClassAsync(className);
            if (!deleteResponse)
                return false;
            var addResponse = await AddClassAsync(className);
            return addResponse;
        }


        //AddStudentToClassAsync
        public async Task<bool> AddStudentToClassAsync(string className, string firstName, string lastName)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var requestData = new
            {
                ClassName = className,
                FirstName = firstName,
                LastName = lastName
            };
            var response = await _httpClient.PostAsJsonAsync("api/Class/AddStudentToClass", requestData);
            return response.IsSuccessStatusCode;
        }

        //GetStudentsInClassAsync
        public async Task<List<StudentModel>> GetStudentsInClassAsync(string className)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetFromJsonAsync<List<StudentModel>>($"api/Class/GetStudentsInClass/{className}");
            return response;

        }

        //GetAllStudentsAsync
        public async Task<List<StudentModel>> GetAllStudentsAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetFromJsonAsync<List<StudentModel>>($"api/Class/GetAllStudents");
            return response;
        }


        //RemoveStudentFromClassAsync
        public async Task<bool> RemoveStudentFromClassAsync(string className, string firstName, string lastName)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var requestData = new
            {
                ClassName = className,
                FirstName = firstName,
                LastName = lastName
            };
            var response = await _httpClient.PostAsJsonAsync("api/Class/RemoveStudentFromClass", requestData);
            return response.IsSuccessStatusCode;
        }

    }
}
