using Microsoft.AspNetCore.Mvc;

namespace SIMS.Controllers.Teacher
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Teacher/Index.cshtml");
        }
    }
}
