using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class PublicProfileController : Controller
    {
        private readonly UserService _userService;
        private readonly BlogService _blogService;

        public PublicProfileController(
            UserService userService,
            BlogService blogService
        )
        {
            _userService = userService;
            _blogService = blogService;
        }

        public async Task<IActionResult> Index()
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            PublicProfileViewModel? publicProfile = new PublicProfileViewModel
            {
                UserName = user.UserName!,
                Name = user.Name!,
                Avatar = user.Avatar!,
                Email = user.ShowEmail ? user.Email : null,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                RecentBlogs = await _blogService.GetRecentBlogsByUser(user.Id, count: 5),

                // test
                Website = "https://example.com",
                Github = "https://github.com/minhtuan622",
                Twitter = "https://twitter.com/username",
                Instagram = "https://instagram.com/nguyenminhtuan622",
                Facebook = "https://facebook.com/minhtuan622",
                RecentActivities = new RecentActivities
                {
                    CommentPercentage = 80,
                    LikePercentage = 72,
                    SharePercentage = 89,
                    RatingPercentage = 55,
                    FollowPercentage = 66
                },
            };
            return View(publicProfile);
        }
    }
}