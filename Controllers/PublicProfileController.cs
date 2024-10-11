using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class PublicProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;

        public PublicProfileController(UserManager<User> userManager, UserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IActionResult> Index(PublicProfileViewModel viewModel)
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            //PublicProfileViewModel? publicProfile = _userService.CreatePublicProfileViewModel(user);
            PublicProfileViewModel? publicProfile = CreatePublicProfileViewModel(user);

            return View(publicProfile);
        }

        private PublicProfileViewModel CreatePublicProfileViewModel(User user)
        {
            PublicProfileViewModel? viewModel = new PublicProfileViewModel
            {
                UserName = user.UserName,
                Name = user.Name,
                Avatar = user.Avatar,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Website = "https://example.com",
                Github = "https://github.com/username",
                Twitter = "https://twitter.com/username",
                Instagram = "https://instagram.com/username",
                Facebook = "https://facebook.com/username",
                RecentPosts = new List<RecentPost>
                {
                    new RecentPost { Title = "Bài viết 1", ViewPercentage = 80 },
                    new RecentPost { Title = "Bài viết 2", ViewPercentage = 72 },
                    new RecentPost { Title = "Bài viết 3", ViewPercentage = 89 },
                    new RecentPost { Title = "Bài viết 4", ViewPercentage = 55 },
                    new RecentPost { Title = "Bài viết 5", ViewPercentage = 66 }
                },
                RecentActivities = new RecentActivities
                {
                    CommentPercentage = 80,
                    LikePercentage = 72,
                    SharePercentage = 89,
                    RatingPercentage = 55,
                    FollowPercentage = 66
                }
            };

            // Thêm logic để lấy dữ liệu thực tế từ cơ sở dữ liệu hoặc các service khác
            // Ví dụ:
            // viewModel.RecentPosts = await _postService.GetRecentPostsForUser(user.Id);
            // viewModel.RecentActivities = await _activityService.GetRecentActivitiesForUser(user.Id);

            return viewModel;
        }
    }
}