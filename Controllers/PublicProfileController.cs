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
        private readonly FriendService _friendService;

        public PublicProfileController(
            UserService userService,
            BlogService blogService,
            FriendService friendService
        )
        {
            _userService = userService;
            _blogService = blogService;
            _friendService = friendService;
        }

        public async Task<IActionResult> Index(string? username)
        {
            User? currentUser = await _userService.GetCurrentUserAsync();
            User? user = await _userService.GetUserAsyncByUserName(username);
            if (user is null || currentUser is null)
            {
                return NotFound();
            }

            PublicProfileViewModel? publicProfile = new PublicProfileViewModel
            {
                UserId = user.Id,
                UserName = user.UserName!,
                Name = user.Name!,
                Avatar = user.Avatar!,
                Email = user.ShowEmail ? user.Email : null,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                RecentBlogs = await _blogService.GetBlogsByUserId(user.Id, count: 6),
                Users = await _userService.GetUsersAsync(),
                IsFriend = _friendService.AreFriends(currentUser.Id, user.Id),
                IsSendRequest = _friendService.Find(user.Id, currentUser!.Id) != null,

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