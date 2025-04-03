using Microsoft.AspNetCore.Mvc;

namespace SIMS.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Admin/Index.cshtml");
        }

    }
}
