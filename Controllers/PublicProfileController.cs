using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class PublicProfileController : Controller
    {
        private readonly UserService _userService;

        public PublicProfileController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            PublicProfileViewModel? publicProfile = await _userService.NewPublicProfileViewModel(user);
            return View(publicProfile);
        }
    }
}