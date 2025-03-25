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
        public IActionResult DashboardCourse()
        {
            var courses = _courseFacade.GetAllCourses();
            return View(courses);
        }

        // Tạo khóa học mới - GET
        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        // Tạo khóa học mới - POST
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

        [HttpGet]
        public IActionResult UpdateCourse(int id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCourse(Course course)
        {
            if (course == null)
            {
                _logger.LogWarning("Course model is null when updating.");
                TempData["ErrorMessage"] = "Invalid course data.";
                return BadRequest("Invalid course data.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is NOT valid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation Error: {ErrorMessage}", error.ErrorMessage);
                }
                TempData["ErrorMessage"] = "Failed to update course. Please fix the errors.";
                return View(course);
            }

            try
            {
                _courseFacade.UpdateCourse(course);
                TempData["SuccessMessage"] = "Course updated successfully!";
                _logger.LogInformation("Course with ID {Id} updated successfully", course.CourseId);
                return RedirectToAction(nameof(DashboardCourse));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course with ID {Id}", course.CourseId);
                TempData["ErrorMessage"] = "An error occurred while updating the course.";
                return View(course);
            }
        }
        // Xóa khóa học - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var course = _courseFacade.GetCourseById(id);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {Id} not found for deletion", id);
                return NotFound();
            }

            _courseFacade.DeleteCourse(id);
            _logger.LogInformation("Course with ID {Id} deleted", id);

            return RedirectToAction(nameof(DashboardCourse));
        }
    }
}
