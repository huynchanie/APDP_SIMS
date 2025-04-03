using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIMS.Data;
using SIMS.Models;
using SIMS.Facades;
using System.Security.Claims;

namespace SIMS.Controllers.Teacher
{
    public class GradeController : Controller
    {
        private readonly DataContext _context;
        private readonly IGradeFacade _gradeFacade;

        public GradeController(DataContext context, IGradeFacade gradeFacade)
        {
            _context = context;
            _gradeFacade = gradeFacade;
        }
        public IActionResult Index()
        {
            // Lấy tất cả điểm từ cơ sở dữ liệu
            var grades = _context.Grades.ToList();
            return View(grades);
        }

        // Hiển thị form nhập điểm
        public IActionResult AssignGrade()
        {
            ViewBag.Students = new SelectList(_context.Users.Where(u => u.RoleId == 3), "UserId", "FullName");
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");

            var teacherName = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "Teacher")
            {
                ViewBag.TeacherName = teacherName;
                return View("AssignGrade");
            }

            ViewBag.ErrorMessage = "Không phải giảng viên!";
            return View("Error");
        }

        // Lưu điểm
        [HttpPost]
        public async Task<IActionResult> AssignGrade(Grade grade)
        {
            if (ModelState.IsValid)
            {
                var result = await _gradeFacade.AssignGradeAsync(grade.UserId, grade.CourseId, grade.Score, grade.Comment);
                if (result)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Lưu điểm thất bại.";
            }

            ViewBag.Students = new SelectList(_context.Users.Where(u => u.RoleId == 3), "UserId", "FullName");
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewBag.TeacherName = User.FindFirstValue(ClaimTypes.Name);

            return View("AssignGrade", grade);
        }
    }
}
