using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SIMS.Facades;
using System.Security.Claims;

namespace SIMS.Controllers
{
    public class AccountController: Controller
    {
        private readonly IUserFacade _userFacade;

        public AccountController(IUserFacade userFacade)
        {
            _userFacade = userFacade;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml");

        }

        [HttpPost]
        public IActionResult Register(string fullName, string email, string password, int roleId, string address, string phoneNumber)
        {
            string result = _userFacade.RegisterUser(fullName, email, password, roleId, address, phoneNumber);

            if (result == "User registered successfully!")
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.ErrorMessage = result;
                return View();
            }
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");

        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _userFacade.LoginUser(email, password);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Unknown") // Gán vai trò
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                switch (user.RoleId)
                {
                    case 1: return RedirectToAction("Index", "Home");
                    case 2: return RedirectToAction("Index", "Home");
                    case 3: return RedirectToAction("Index", "Home");
                    default: return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password!";
            return View();
        }


    }

}

