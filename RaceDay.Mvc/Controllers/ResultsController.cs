using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ResultService _resultService;

        public ResultsController(ResultService resultService)
        {
            _resultService = resultService;
        }

        // Check if the logged-in user is an Organizer
        private bool IsOrganizer()
        {
            return HttpContext.Session.GetString("UserRole") == "Organizer"
                && !string.IsNullOrEmpty(
                    HttpContext.Session.GetString("JWToken"));
        }

        // Check if the logged-in user is a Participant
        private bool IsParticipant()
        {
            return HttpContext.Session.GetString("UserRole") == "Participant"
                && !string.IsNullOrEmpty(
                    HttpContext.Session.GetString("JWToken"));
        }

        // View results for a specific event
        public async Task<IActionResult> EventResults(int eventID)
        {
            if (eventID <= 0)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var results =
                await _resultService.GetEventResultsAsync(eventID);

            ViewBag.EventID = eventID;

            return View(results);
        }

        // View results belonging to the logged-in Participant
        public async Task<IActionResult> MyResults()
        {
            if (!IsParticipant())
            {
                return RedirectToAction("Login", "Account");
            }

            string token = HttpContext.Session.GetString("JWToken")!;

            var results =
                await _resultService.GetMyResultsAsync(token);

            return View(results);
        }

        // Open the page for capturing results
        [HttpGet]
        public IActionResult CaptureResults(int eventID, int enrolmentID)
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            if (eventID <= 0 || enrolmentID <= 0)
            {
                return RedirectToAction(
                    "OrganizerEnrolments", "Enrolments");
            }

            ViewBag.EventID = eventID;

            return View(new ResultRequest
            {
                EnrolmentID = enrolmentID
            });
        }

        // Save a new result
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CaptureResults(
            int eventID, ResultRequest request)
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid ||
                eventID <= 0 ||
                request.EnrolmentID <= 0 ||
                request.FinishTime < TimeSpan.Zero ||
                request.FinishingPosition <= 0)
            {
                ViewBag.EventID = eventID;
                ViewBag.Error = "Please enter valid result details.";

                return View(request);
            }

            string token = HttpContext.Session.GetString("JWToken")!;

            var response = await _resultService.CreateResultAsync(
                eventID, request, token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Result recorded successfully!";

                return RedirectToAction(
                    "EventResults", new { eventID });
            }

            ViewBag.EventID = eventID;

            if ((int)response.StatusCode == 409)
            {
                ViewBag.Error =
                    "A result already exists for this enrolment.";
            }
            else
            {
                ViewBag.Error =
                    "Could not record the result. Please check the details.";
            }

            return View(request);
        }
    }
}