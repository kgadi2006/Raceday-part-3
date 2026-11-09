
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RaceDayApi");
        }

        // Display Login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Send Login details to API
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/login", model);

            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password.";
                return View(model);
            }

            using var document = JsonDocument.Parse(result);

            string token = document.RootElement
                .GetProperty("token").GetString()!;

            HttpContext.Session.SetString("JWToken", token);

            // Save the user's role in the session
            string role = document.RootElement
                .GetProperty("user")
                .GetProperty("role")
                .GetString()!;

            HttpContext.Session.SetString("UserRole", role);

            return RedirectToAction("Index", "Dashboard");
        }

        // Display Register page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Send Registration details to API
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/register", model);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Registration failed. Please check your details.";
                return View(model);
            }

            TempData["Success"] = "Registration successful. Please log in.";

            return RedirectToAction("Login");
        }
    }
}
