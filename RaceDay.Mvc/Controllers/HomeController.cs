
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Diagnostics;

namespace RaceDay.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EventService _eventService;

        public HomeController(
            ILogger<HomeController> logger,
            EventService eventService)
        {
            _logger = logger;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetEventsAsync();

            // Get the logged-in user's role
            ViewBag.Role = HttpContext.Session.GetString("UserRole");

            return View(events);
        }
        public async Task<IActionResult> Details(int id)
        {
            var raceEvent = await _eventService.GetEventByIdAsync(id);

            if (raceEvent == null)
            {
                return NotFound();
            }

            return View(raceEvent);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
