using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Facades;
using SIMS.Models;
using System;

namespace SIMS.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentFacade _enrollmentFacade;
        private readonly DataContext _context;

        public EnrollmentController(IEnrollmentFacade enrollmentFacade, DataContext context)
        {
            _enrollmentFacade = enrollmentFacade ?? throw new ArgumentNullException(nameof(enrollmentFacade));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        // GET: /Enrollment/Index
        public IActionResult Index()
        {
            var enrollments = _enrollmentFacade.GetAllEnrollments();
            return View(enrollments);
        }

        // GET: /Enrollment/Details/{id}
        public IActionResult Details(int id)
        {
            var enrollment = _enrollmentFacade.GetEnrollmentById(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return View(enrollment);
        }
        public IActionResult CreateEnrollment()
        {
            ViewBag.Students = new SelectList(_context.Users.Where(u => u.RoleId == 3), "UserId", "FullName");
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
            return View();
        }
       
        

        // POST: /Enrollment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEnrollment(Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _enrollmentFacade.CreateEnrollment(enrollment);
                TempData["SuccessMessage"] = "Đăng ký cho sinh viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(enrollment);
        }

        // GET: /Enrollment/Edit/{id}
        public IActionResult Edit(int id)
        {
            var enrollment = _enrollmentFacade.GetEnrollmentById(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return View(enrollment);
        }

        // POST: /Enrollment/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _enrollmentFacade.UpdateEnrollment(enrollment);
                return RedirectToAction(nameof(Index));
            }
            return View(enrollment);
        }

        // GET: /Enrollment/Delete/{id}
        public IActionResult Delete(int id)
        {
            var enrollment = _enrollmentFacade.GetEnrollmentById(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return View(enrollment);
        }

        // POST: /Enrollment/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(int id)
        {
            _enrollmentFacade.DeleteEnrollment(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
