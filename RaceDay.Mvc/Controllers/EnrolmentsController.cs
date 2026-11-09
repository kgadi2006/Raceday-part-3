using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly EnrolmentService _enrolmentService;
        private readonly EventService _eventService;
        private readonly CategoryService _categoryService;

        public EnrolmentsController(
            EnrolmentService enrolmentService,
            EventService eventService,
            CategoryService categoryService)
        {
            _enrolmentService = enrolmentService;
            _eventService = eventService;
            _categoryService = categoryService;
        }

        // Check whether the user is a Participant
        private bool IsParticipant()
        {
            string? token = HttpContext.Session.GetString("JWToken");
            string? role = HttpContext.Session.GetString("UserRole");

            return !string.IsNullOrEmpty(token) && role == "Participant";
        }

        // Display the participant's enrolments
        public async Task<IActionResult> Index()
        {
            if (!IsParticipant())
            {
                return RedirectToAction("Login", "Account");
            }

            string token = HttpContext.Session.GetString("JWToken")!;

            var enrolments =
                await _enrolmentService.GetMyEnrolmentsAsync(token);

            return View(enrolments);
        }

        // Display the enrolment form
        public async Task<IActionResult> Create(int eventID)
        {
            if (!IsParticipant())
            {
                return RedirectToAction("Login", "Account");
            }

            if (eventID <= 0)
            {
                return BadRequest("Invalid event ID.");
            }

            var raceEvent =
                await _eventService.GetEventByIdAsync(eventID);

            if (raceEvent == null)
            {
                return NotFound();
            }

            var categories =
                await _categoryService.GetCategoriesAsync(eventID);

            ViewBag.EventName = raceEvent.EventName;
            ViewBag.Categories = categories;
            ViewBag.EventID = eventID;

            return View();
        }

        // Submit a new enrolment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventID, int categoryID)
        {
            if (!IsParticipant())
            {
                return RedirectToAction("Login", "Account");
            }

            string token = HttpContext.Session.GetString("JWToken")!;

            var response =
                await _enrolmentService.CreateEnrolmentAsync(
                    eventID, categoryID, token);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    "Unable to enrol. You may already be enrolled or the selected category may be invalid.";

                return RedirectToAction("Create", new { eventID });
            }

            TempData["Success"] = "You have successfully enrolled!";

            return RedirectToAction("Index");
        }

        // Cancel an enrolment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsParticipant())
            {
                return RedirectToAction("Login", "Account");
            }

            string token = HttpContext.Session.GetString("JWToken")!;

            var response =
                await _enrolmentService.DeleteEnrolmentAsync(id, token);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Unable to cancel enrolment.";
            }
            else
            {
                TempData["Success"] =
                    "Your enrolment has been cancelled.";
            }

            return RedirectToAction("Index");
        }
        // View enrolments belonging to the logged-in Organizer
        public async Task<IActionResult> OrganizerEnrolments()
        {
            string? token = HttpContext.Session.GetString("JWToken");
            string? role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token) || role != "Organizer")
            {
                return RedirectToAction("Login", "Account");
            }

            var enrolments =
                await _enrolmentService.GetOrganizerEnrolmentsAsync(token);

            return View(enrolments);
        }
        
    }
}