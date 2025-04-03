using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Facades;
using SIMS.Models;
using System;

namespace SIMS.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentFacade _enrollmentFacade;
        private readonly DataContext _context;

        public EnrollmentController(IEnrollmentFacade enrollmentFacade, DataContext context)
        {
            _enrollmentFacade = enrollmentFacade;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var enrollments = await _enrollmentFacade.GetAllEnrollmentsAsync();
            return View(enrollments);
        }

        public IActionResult CreateEnrollment()
        {
            ViewBag.Students = new SelectList(_context.Users.Where(u => u.RoleId == 3), "UserId", "FullName");
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment(int studentId, int courseId, string status)
        {
            if (await _enrollmentFacade.AssignStudentToCourseAsync(studentId, courseId, status))
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Gán sinh viên vào khóa học thất bại.");

            // Load lại dữ liệu cho ViewBag
            ViewBag.Students = new SelectList(_context.Users.Where(u => u.RoleId == 3), "UserId", "FullName");
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");

            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _enrollmentFacade.RemoveEnrollmentAsync(id);
            return RedirectToAction("Index");
        }
    }

}