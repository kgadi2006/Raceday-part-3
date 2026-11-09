using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Check whether the user is an Organizer
        private bool IsOrganizer()
        {
            string? token = HttpContext.Session.GetString("JWToken");
            string? role = HttpContext.Session.GetString("UserRole");

            return !string.IsNullOrEmpty(token) && role == "Organizer";
        }

        // Display categories for a specific event
        public async Task<IActionResult> Index(int eventID)
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            if (eventID <= 0)
            {
                return BadRequest("Invalid event ID.");
            }

            var categories =
                await _categoryService.GetCategoriesAsync(eventID);

            ViewBag.EventID = eventID;

            return View(categories);
        }

        // Display the Create Category form
        public IActionResult Create(int eventID)
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            if (eventID <= 0)
            {
                return BadRequest("Invalid event ID.");
            }

            var category = new Category
            {
                EventID = eventID
            };

            return View(category);
        }

        // Submit a new category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("Login", "Account");
            }

            string token = HttpContext.Session.GetString("JWToken")!;

            var response =
                await _categoryService.CreateCategoryAsync(model, token);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "Unable to create category. Please check your details.";

                return View(model);
            }

            TempData["Success"] = "Category created successfully!";

            return RedirectToAction("Index",
                new { eventID = model.EventID });
        }
    }
}