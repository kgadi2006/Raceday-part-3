
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventService _eventService;

        public EventsController(EventService eventService)
        {
            _eventService = eventService;
        }

        // Check whether the user is an Organizer
        private bool IsOrganizer()
        {
            string? token = HttpContext.Session.GetString("JWToken");
            string? role = HttpContext.Session.GetString("UserRole");

            return !string.IsNullOrEmpty(token) && role == "Organizer";
        }

        // Display all events
        public async Task<IActionResult> Index()
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            var events = await _eventService.GetEventsAsync();

            return View(events);
        }

        // Display the Create Event form
        public IActionResult Create()
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // Submit a new event
        [HttpPost]
        public async Task<IActionResult> Create(Event model)
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            string token = HttpContext.Session.GetString("JWToken")!;

            var response = await _eventService.CreateEventAsync(model, token);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to create event. Please check your details.";
                return View(model);
            }

            TempData["Success"] = "Event created successfully!";

            return RedirectToAction("Index");
        }
        // Display the Edit Event form
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            var raceEvent = await _eventService.GetEventByIdAsync(id);

            if (raceEvent == null)
            {
                return NotFound();
            }

            return View(raceEvent);
        }

        // Submit the updated event
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Event model)
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            string token = HttpContext.Session.GetString("JWToken")!;

            var response = await _eventService.UpdateEventAsync(id, model, token);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to update event. Please check your details.";
                return View(model);
            }

            TempData["Success"] = "Event updated successfully!";

            return RedirectToAction("Index");
        }
        // Display the Delete confirmation page
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            var raceEvent = await _eventService.GetEventByIdAsync(id);

            if (raceEvent == null)
            {
                return NotFound();
            }

            return View(raceEvent);
        }

        // Confirm and delete the event
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            string token = HttpContext.Session.GetString("JWToken")!;

            var response = await _eventService.DeleteEventAsync(id, token);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Unable to delete event.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Event deleted successfully!";

            return RedirectToAction("Index");
        }

    }
}
