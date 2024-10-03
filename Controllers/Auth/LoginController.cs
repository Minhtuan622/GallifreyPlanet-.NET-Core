using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Controllers.Auth
{
    public class LoginController : Controller
    {
        private readonly GallifreyPlanetContext _context;

        public LoginController(GallifreyPlanetContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User user)
        {
            if (ModelState.IsValid)
            {
                User? ahihi = _context.User.SingleOrDefault(m => m.Phone == user.Phone && m.Password == user.Password);

                if (ahihi != null)
                {
                    HttpContext.Session.SetString("userId", user.Name);
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Đăng nhập không thành công.");
            return View(user);
        }
    }
}
