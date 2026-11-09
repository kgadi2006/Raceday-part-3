using System.Net.Http.Json;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class CategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get categories for a specific event
        public async Task<List<Category>> GetCategoriesAsync(int eventID)
        {
            var response = await _httpClient.GetAsync(
                $"api/events/{eventID}/categories");

            response.EnsureSuccessStatusCode();

            var categories =
                await response.Content.ReadFromJsonAsync<List<Category>>();

            return categories ?? new List<Category>();
        }

        // Create a new category
        public async Task<HttpResponseMessage> CreateCategoryAsync(
            Category category, string token)
        {
            var request = new
            {
                category.EventID,
                category.CategoryName,
                category.CategoryType,
                category.CategoryValue
            };

            using var message = new HttpRequestMessage(
                HttpMethod.Post, "api/categories");

            message.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            message.Content = JsonContent.Create(request);

            return await _httpClient.SendAsync(message);
        }
    }
}