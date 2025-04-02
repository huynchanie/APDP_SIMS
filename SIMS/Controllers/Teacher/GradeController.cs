using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIMS.Data;
using SIMS.Models;
using System.Linq;
using System.Security.Claims;

namespace SIMS.Controllers.Teacher
{
    public class GradeController : Controller
    {
        private readonly DataContext _context;

        public GradeController(DataContext context)
        {
            _context = context;
        }

       
      
        // Hiển thị form nhập điểm
        public IActionResult AssignGrade()
        {
            // Lấy danh sách sinh viên
            ViewBag.Students = new SelectList(_context.Users.Where(u => u.RoleId == 3), "UserId", "FullName");

            // Lấy danh sách khóa học
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");

            // Lấy thông tin giảng viên đang đăng nhập từ Claims
            var teacherName = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);

            // Kiểm tra nếu người dùng là giảng viên
            if (role == "Teacher")
            {
                ViewBag.TeacherName = teacherName;
                return View();  // Trả về view trong thư mục Grade
            }
            else
            {
                // Nếu người dùng không phải giảng viên
                ViewBag.ErrorMessage = "Không phải giảng viên!";
                return View("Error");
            }
        }

        // Lưu dữ liệu điểm vào database
        [HttpPost]
        public IActionResult AssignGrade(Grade grade)
        {
            if (ModelState.IsValid)
            {
                // Thêm điểm vào database
                _context.Grades.Add(grade);
                _context.SaveChanges();

                // Chuyển hướng tới trang danh sách điểm
                return RedirectToAction("Index");
            }

            // Nếu có lỗi trong form, load lại danh sách sinh viên và khóa học
            ViewBag.Students = new SelectList(_context.Users.Where(u => u.RoleId == 3), "UserId", "FullName");
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");

            // Trả về lại form nhập điểm
            return View("Grade/AssignGrade", grade);
        }
    }
}
