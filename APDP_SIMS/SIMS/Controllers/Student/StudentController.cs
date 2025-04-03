using Microsoft.AspNetCore.Mvc;

namespace SIMS.Controllers.Student
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Student/Index.cshtml");
        }
    }
}
