using System.Net.Http.Json;
using System.Net.Http.Headers;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class ResultService
    {
        private readonly HttpClient _httpClient;

        public ResultService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get results for a specific event
        public async Task<List<Result>> GetEventResultsAsync(int eventID)
        {
            var results = await _httpClient.GetFromJsonAsync<List<Result>>(
                $"api/events/{eventID}/results");

            return results ?? new List<Result>();
        }

        // Get results for the logged-in Participant
        public async Task<List<Result>> GetMyResultsAsync(string token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/results/me");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var results = await response.Content.ReadFromJsonAsync<List<Result>>();

            return results ?? new List<Result>();
        }

        // Create a new result
        public async Task<HttpResponseMessage> CreateResultAsync(
            int eventID, ResultRequest requestData, string token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, $"api/events/{eventID}/results");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(requestData);

            return await _httpClient.SendAsync(request);
        }

        // Update an existing result
        public async Task<HttpResponseMessage> UpdateResultAsync(
            int resultID, ResultRequest requestData, string token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put, $"api/results/{resultID}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(requestData);

            return await _httpClient.SendAsync(request);
        }
    }
}