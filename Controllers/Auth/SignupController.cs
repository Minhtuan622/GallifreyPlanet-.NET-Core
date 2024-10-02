using Microsoft.AspNetCore.Mvc;
using GallifreyPlanet.Models;
using System.Security.Cryptography;
using System.Text;

namespace GallifreyPlanet.Controllers.Auth
{
    public class SignupController : Controller
    {
        // GET: /Signup
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User user)
        {
            if (ModelState.IsValid)
            {
                // TODO: Check if email already exists in the database

                // Hash the password
                user.Password = HashPassword(user.Password);

                // TODO: Save user to database

                // Redirect to login page
                return RedirectToAction("Index", "Login");
            }

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
