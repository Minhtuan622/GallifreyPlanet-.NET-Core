using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class PublicProfileController(
        UserService userService,
        BlogService blogService,
        FriendService friendService)
        : Controller
    {
        public async Task<IActionResult> Index(string? username)
        {
            User? currentUser = await userService.GetCurrentUserAsync();
            User? profileUser = await userService.GetUserAsyncByUserName(username!);
            if (profileUser is null || currentUser is null)
            {
                return NotFound();
            }

            PublicProfileViewModel publicProfile = new PublicProfileViewModel
            {
                UserId = profileUser.Id,
                UserName = profileUser.UserName!,
                Name = profileUser.Name!,
                Avatar = profileUser.Avatar!,
                Email = profileUser.ShowEmail ? profileUser.Email : null,
                Address = profileUser.Address,
                PhoneNumber = profileUser.PhoneNumber,
                RecentBlogs = await blogService.GetBlogsByUserId(profileUser.Id, count: 6),
                Friends = await friendService.GetFriends(profileUser.Id),
                IsFriend = friendService.AreFriends(currentUser.Id, profileUser.Id),
                IsSendRequest = friendService.Find(profileUser.Id, currentUser!.Id) != null,
                AllowChat = profileUser.AllowChat,
                AllowAddFriend = profileUser.AllowAddFriend,
                CurrentUser = currentUser,

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