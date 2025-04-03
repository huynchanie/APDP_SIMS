using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SIMS.Facades;
using SIMS.Models;

namespace SIMS.Controllers.Admin
{
    public class CourseController : Controller
    {
        private readonly ICourseFacade _courseFacade;
        private readonly ILogger<CourseController> _logger;

        public CourseController(ICourseFacade courseFacade, ILogger<CourseController> logger)
        {
            _courseFacade = courseFacade;
            _logger = logger;
        }

        // Hiển thị danh sách khóa học
        [Authorize(Roles = "Admin,Teacher,Student")]
        public IActionResult DashboardCourse()
        {
            var courses = _courseFacade.GetAllCourses();
            if (User.IsInRole("Admin"))
            {
                ViewData["Layout"] = "~/Views/Shared/_LayoutHeader_Admin.cshtml";
            }
            else if (User.IsInRole("Teacher"))
            {
                ViewData["Layout"] = "~/Views/Shared/_LayoutHeader_Teacher.cshtml";
            }
            else if (User.IsInRole("Student"))
            {
                ViewData["Layout"] = "~/Views/Shared/_LayoutHeader_Student.cshtml";
            }
            return View(courses);
        }

        // Tạo khóa học mới - GET
        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        // Tạo khóa học mới - POST
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCourse(Course course)
        {
            if (course == null)
            {
                _logger.LogWarning("Course model is null when creating.");
                return BadRequest("Invalid course data.");
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model is valid. Saving course: {CourseName}", course.CourseName);
                _courseFacade.CreateCourse(course);
                return RedirectToAction(nameof(DashboardCourse));
            }
            else
            {
                _logger.LogWarning("ModelState is NOT valid!");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation Error: {ErrorMessage}", error.ErrorMessage);
                }
            }
            return View(course);
        }
        [Authorize(Roles = "Admin,Teacher")]
        // Chỉnh sửa khóa học - GET (Đã kết hợp với EditCourse)
        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            var course = _courseFacade.GetCourseById(id);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {Id} not found for editing", id);
                return NotFound();
            }
            return View(course);
        }

        // Chỉnh sửa khóa học - POST
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCourse(Course course)
        {
            if (course == null)
            {
                _logger.LogWarning("Course model is null when updating.");
                return BadRequest("Invalid course data.");
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model is valid. Saving course: {CourseName}", course.CourseName);
                _courseFacade.EditCourse(course);
                return RedirectToAction(nameof(DashboardCourse));
            }
            else
            {
                _logger.LogWarning("ModelState is NOT valid!");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation Error: {ErrorMessage}", error.ErrorMessage);
                }
                return View(course);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DeleteCourse(int id)
        {
            var course = _courseFacade.GetCourseById(id);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {Id} not found for deletion", id);
                return NotFound();
            }
            return View(course);
        }

        [HttpPost, ActionName("DeleteCourse")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDeleteCourse(int id)
        {
            var course = _courseFacade.GetCourseById(id);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {Id} not found when confirming deletion", id);
                return NotFound();
            }

            _courseFacade.DeleteCourse(id);
            TempData["SuccessMessage"] = $"Course '{course.CourseName}' deleted successfully!";
            _logger.LogInformation("Course with ID {Id} deleted successfully", id);

            return RedirectToAction(nameof(DashboardCourse));
        }

    }
}
