using Microsoft.AspNetCore.Mvc;
using GallifreyPlanet.Models;
using System.Security.Cryptography;
using System.Text;

namespace GallifreyPlanet.Controllers.Auth
{
    public class LoginController : Controller
    {
        // GET: /Login
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User user)
        {
            if (ModelState.IsValid)
            {
                // TODO: Check user credentials against database
                // For now, we'll just simulate a successful login
                
                // If login is successful, redirect to home page
                return RedirectToAction("Index", "Home");
            }

            // If login fails, show error message
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(user);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
