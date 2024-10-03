using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Web.Helpers;

namespace GallifreyPlanet.Controllers.Auth
{
    public class SignupController : Controller
    {
        private readonly GallifreyPlanetContext _context;

        public SignupController(GallifreyPlanetContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup([Bind("Id,Name,Phone,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                // TODO: Check if phone already exists in the database

                if (user.Password != null)
                {
                    user.Password = Crypto.HashPassword(user.Password);
                }
                else
                {
                    ModelState.AddModelError("", "Chưa nhập mật khẩu.");
                    return View(user);
                }

                _context.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Index", "Login");
            }

            return View(user);
        }
    }
}
