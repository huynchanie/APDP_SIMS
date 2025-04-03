using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIMS.Data;
using SIMS.Models;
using SIMS.Facades;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                // Get the current user ID
                var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacherName = User.FindFirstValue(ClaimTypes.Name);
                
                // Lấy tất cả điểm từ cơ sở dữ liệu với include User và Course
                var grades = _context.Grades
                    .Include(g => g.User)
                    .Include(g => g.Course)
                    .ToList();
                    
                ViewBag.TeacherName = teacherName ?? "Giảng viên";
                ViewBag.GradeCount = grades.Count;
                
                return View(grades);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh sách điểm: {ex.Message}";
                return View(new List<Grade>());
            }
        }

        // Hiển thị form nhập điểm
        public async Task<IActionResult> AssignGrade()
        {
            try 
            {
                // Lấy tất cả sinh viên (RoleId = 3)
                var students = await _context.Users
                    .Where(u => u.RoleId == 3)
                    .ToListAsync();
                    
                // Lấy tất cả khóa học
                var courses = await _context.Courses.ToListAsync();
                    
                ViewBag.Students = new SelectList(students, "UserId", "FullName");
                ViewBag.Courses = new SelectList(courses, "CourseId", "CourseName");
                
                // Hiện thông tin số lượng đăng ký
                var enrollmentCount = await _context.Enrollments.CountAsync();
                ViewBag.EnrollmentCount = enrollmentCount;
                ViewBag.StudentCount = students.Count;
                ViewBag.CourseCount = courses.Count;

                var teacherName = User.FindFirstValue(ClaimTypes.Name);
                var role = User.FindFirstValue(ClaimTypes.Role);

                if (string.IsNullOrEmpty(role) || role == "Teacher")
                {
                    ViewBag.TeacherName = teacherName ?? "Giảng viên";
                    return View("AssignGrade");
                }

                ViewBag.ErrorMessage = "Không phải giảng viên!";
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi: {ex.Message}";
                return View("Error");
            }
        }

        // Lưu điểm
        [HttpPost]
        public async Task<IActionResult> AssignGrade(Grade grade)
        {
            try
            {
                // Debug info to track incoming values
                ViewBag.DebugInfo = $"Received: UserId={grade.UserId}, CourseId={grade.CourseId}, Score={grade.Score}";
                
                // Manually reset ModelState for UserId and CourseId since we have the IDs
                if (grade.UserId > 0)
                {
                    ModelState.Remove("User");
                    ModelState.Remove("UserId");
                }
                
                if (grade.CourseId > 0)
                {
                    ModelState.Remove("Course");
                    ModelState.Remove("CourseId");
                }
                
                // Re-validate model state after removing navigational properties validation
                if (ModelState.IsValid)
                {
                    // Validation logic for score
                    if (grade.Score < 0 || grade.Score > 100)
                    {
                        ViewBag.ErrorMessage = "Điểm phải nằm trong khoảng từ 0 đến 100.";
                    }
                    else
                    {
                        // Simple validation first
                        if (grade.UserId <= 0 || grade.CourseId <= 0)
                        {
                            ViewBag.ErrorMessage = "Vui lòng chọn sinh viên và khóa học.";
                        }
                        else
                        {
                            // Check if student is enrolled in the course first
                            var enrollment = await _context.Enrollments
                                .FirstOrDefaultAsync(e => e.StudentId == grade.UserId && e.CourseId == grade.CourseId);
                            
                            if (enrollment == null)
                            {
                                // Option to auto-enroll the student before grading
                                var autoEnroll = true; // Set to true to automatically enroll students

                                if (autoEnroll)
                                {
                                    var newEnrollment = new Enrollment
                                    {
                                        StudentId = grade.UserId,
                                        CourseId = grade.CourseId,
                                        Status = "Approved", // Automatically approve enrollment
                                        EnrolledAt = DateTime.UtcNow
                                    };

                                    try
                                    {
                                        _context.Enrollments.Add(newEnrollment);
                                        await _context.SaveChangesAsync();
                                        
                                        // Now try to assign the grade
                                        if (grade.Comment == null)
                                        {
                                            grade.Comment = string.Empty;
                                        }
                                        
                                        var result = await _gradeFacade.AssignGradeAsync(
                                            grade.UserId,
                                            grade.CourseId,
                                            grade.Score,
                                            grade.Comment);

                                        if (result)
                                        {
                                            TempData["SuccessMessage"] = "Đã tự động đăng ký khóa học và lưu điểm thành công.";
                                            return RedirectToAction("Index");
                                        }
                                        else
                                        {
                                            ViewBag.ErrorMessage = "Đã đăng ký khóa học nhưng không thể lưu điểm. Vui lòng thử lại.";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ViewBag.ErrorMessage = $"Không thể tự động đăng ký: {ex.Message}";
                                        ViewBag.ErrorDetail = ex.InnerException?.Message;
                                    }
                                }
                                else
                                {
                                    // Log and alert if student is not enrolled
                                    ViewBag.ErrorMessage = "Sinh viên chưa đăng ký khóa học này. Vui lòng đăng ký trước khi gán điểm.";
                                    ViewBag.DebugInfo = $"Không tìm thấy đăng ký cho sinh viên {grade.UserId} và khóa học {grade.CourseId}";
                                }
                            }
                            else
                            {
                                // Try direct database save as fallback
                                try {
                                    // Check if grade already exists
                                    var existingGrade = await _context.Grades
                                        .FirstOrDefaultAsync(g => g.UserId == grade.UserId && g.CourseId == grade.CourseId);
                                    
                                    if (existingGrade != null)
                                    {
                                        // Update existing grade
                                        existingGrade.Score = grade.Score;
                                        existingGrade.Comment = grade.Comment ?? string.Empty;
                                        existingGrade.CreateTime = DateTime.UtcNow;
                                        
                                        _context.Grades.Update(existingGrade);
                                        var saveResult = await _context.SaveChangesAsync();
                                        
                                        if (saveResult > 0)
                                        {
                                            TempData["SuccessMessage"] = "Đã cập nhật điểm thành công (phương pháp trực tiếp).";
                                            return RedirectToAction("Index");
                                        }
                                    }
                                    else
                                    {
                                        // Create new grade directly
                                        var newGrade = new Grade
                                        {
                                            UserId = grade.UserId,
                                            CourseId = grade.CourseId,
                                            Score = grade.Score,
                                            Comment = grade.Comment ?? string.Empty,
                                            CreateTime = DateTime.UtcNow
                                        };
                                        
                                        _context.Grades.Add(newGrade);
                                        var saveResult = await _context.SaveChangesAsync();
                                        
                                        if (saveResult > 0)
                                        {
                                            TempData["SuccessMessage"] = "Đã lưu điểm thành công (phương pháp trực tiếp).";
                                            return RedirectToAction("Index");
                                        }
                                    }
                                    
                                    ViewBag.ErrorMessage = "Lưu điểm thất bại sau khi thử phương pháp trực tiếp.";
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.ErrorMessage = $"Lỗi khi lưu trực tiếp: {ex.Message}";
                                    ViewBag.ErrorDetail = ex.InnerException?.Message;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Chi tiết lỗi validation
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    ViewBag.ErrorMessage = $"Dữ liệu không hợp lệ: {string.Join(", ", errors)}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
                if (ex.InnerException != null)
                {
                    ViewBag.ErrorDetail = ex.InnerException.Message;
                }
            }

            // Tải lại danh sách
            ViewBag.Students = new SelectList(await _context.Users.Where(u => u.RoleId == 3).ToListAsync(), "UserId", "FullName");
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");
            ViewBag.TeacherName = User.FindFirstValue(ClaimTypes.Name) ?? "Giảng viên";

            return View("AssignGrade", grade);
        }

        // Edit grade (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var grade = await _context.Grades
                .Include(g => g.User)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.GradeId == id);
            
            if (grade == null)
            {
                return NotFound();
            }

            ViewBag.Students = new SelectList(_context.Users.Where(u => u.RoleId == 3), "UserId", "FullName");
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewBag.TeacherName = User.FindFirstValue(ClaimTypes.Name);

            return View(grade);
        }

        // Edit grade (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Grade grade)
        {
            if (id != grade.GradeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _gradeFacade.UpdateGradeAsync(id, grade.Score, grade.Comment);
                    if (result)
                    {
                        TempData["SuccessMessage"] = "Đã cập nhật điểm thành công.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await GradeExistsAsync(grade.GradeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewBag.ErrorMessage = "Cập nhật điểm thất bại. Vui lòng thử lại.";
            }

            ViewBag.Students = new SelectList(_context.Users.Where(u => u.RoleId == 3), "UserId", "FullName");
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewBag.TeacherName = User.FindFirstValue(ClaimTypes.Name);

            return View(grade);
        }

        // Delete grade
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _gradeFacade.DeleteGradeAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã xóa điểm thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa điểm thất bại.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Hiển thị danh sách đăng ký
        public async Task<IActionResult> Enrollments()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
            
            return View(enrollments);
        }

        // Lấy danh sách sinh viên đã đăng ký khóa học
        [HttpGet]
        public async Task<IActionResult> GetEnrolledStudents(int courseId)
        {
            try
            {
                var enrolledStudents = await _context.Enrollments
                    .Where(e => e.CourseId == courseId && (e.Status == "Approved" || e.Status == "Active"))
                    .Include(e => e.Student)
                    .Select(e => new { e.Student.UserId, e.Student.FullName })
                    .Distinct()
                    .ToListAsync();

                return Json(enrolledStudents);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckDatabase()
        {
            var result = new Dictionary<string, object>();
            
            try
            {
                // Check database connectivity
                bool canConnect = _context.Database.CanConnect();
                result.Add("DatabaseConnection", canConnect);
                
                if (canConnect)
                {
                    // Check tables
                    int userCount = await _context.Users.CountAsync();
                    int courseCount = await _context.Courses.CountAsync();
                    int gradeCount = await _context.Grades.CountAsync();
                    int enrollmentCount = await _context.Enrollments.CountAsync();
                    
                    result.Add("UserCount", userCount);
                    result.Add("CourseCount", courseCount);
                    result.Add("GradeCount", gradeCount);
                    result.Add("EnrollmentCount", enrollmentCount);
                    
                    // Check if students exist (RoleId = 3)
                    int studentCount = await _context.Users.Where(u => u.RoleId == 3).CountAsync();
                    result.Add("StudentCount", studentCount);
                    
                    // Get first few users for inspection
                    var users = await _context.Users.Take(5).Select(u => new { u.UserId, u.FullName, u.RoleId }).ToListAsync();
                    result.Add("SampleUsers", users);
                    
                    // Get first few courses for inspection
                    var courses = await _context.Courses.Take(5).Select(c => new { c.CourseId, c.CourseName }).ToListAsync();
                    result.Add("SampleCourses", courses);
                }
                
                return Json(result);
            }
            catch (Exception ex)
            {
                result.Add("Error", ex.Message);
                result.Add("InnerError", ex.InnerException?.Message);
                result.Add("StackTrace", ex.StackTrace);
                return Json(result);
            }
        }

        private async Task<bool> GradeExistsAsync(int id)
        {
            return await _context.Grades.AnyAsync(g => g.GradeId == id);
        }
    }
}
