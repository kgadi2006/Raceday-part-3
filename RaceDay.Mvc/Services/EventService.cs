
using System.Net.Http.Json;
using System.Text.Json;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class EventService
    {
        private readonly HttpClient _httpClient;

        public EventService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get all events
        public async Task<List<Event>> GetEventsAsync()
        {
            var response = await _httpClient.GetAsync("api/events");

            response.EnsureSuccessStatusCode();

            var events = await response.Content.ReadFromJsonAsync<List<Event>>();

            return events ?? new List<Event>();
        }

        // Get one event by ID
        public async Task<Event?> GetEventByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/events/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Event>();
        }

        // Create an event
        public async Task<HttpResponseMessage> CreateEventAsync(Event raceEvent, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("api/events", raceEvent);

            return response;
        }

        // Update an event
        public async Task<HttpResponseMessage> UpdateEventAsync(int id, Event raceEvent, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsJsonAsync($"api/events/{id}", raceEvent);

            return response;
        }

        // Delete an event
        public async Task<HttpResponseMessage> DeleteEventAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/events/{id}");

            return response;
        }
    }
}
