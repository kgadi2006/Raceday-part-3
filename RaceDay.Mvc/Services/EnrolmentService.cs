using System.Net.Http.Json;
using System.Net.Http.Headers;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class EnrolmentService
    {
        private readonly HttpClient _httpClient;

        public EnrolmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get enrolments belonging to the logged-in participant
        public async Task<List<EventEnrolment>> GetMyEnrolmentsAsync(
            string token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/enrolments/me");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var enrolments =
                await response.Content.ReadFromJsonAsync<List<EventEnrolment>>();

            return enrolments ?? new List<EventEnrolment>();
        }

        // Create a new enrolment
        public async Task<HttpResponseMessage> CreateEnrolmentAsync(
            int eventID, int categoryID, string token)
        {
            var requestData = new
            {
                EventID = eventID,
                CategoryID = categoryID
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"api/events/{eventID}/enrolments");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(requestData);

            return await _httpClient.SendAsync(request);
        }

        // Cancel an enrolment
        public async Task<HttpResponseMessage> DeleteEnrolmentAsync(
            int enrolmentID, string token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Delete, $"api/enrolments/{enrolmentID}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return await _httpClient.SendAsync(request);
        }
        // Get enrolments for the logged-in Organizer
        public async Task<List<OrganizerEnrolment>> GetOrganizerEnrolmentsAsync(string token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/organizer/enrolments");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var enrolments =
                await response.Content.ReadFromJsonAsync<List<OrganizerEnrolment>>();

            return enrolments ?? new List<OrganizerEnrolment>();
        }
    }
}