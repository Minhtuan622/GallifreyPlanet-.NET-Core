using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers;

public class PublicProfileController(
    UserService userService,
    BlogService blogService,
    FriendService friendService)
    : Controller
{
    public async Task<IActionResult> Index(string? username)
    {
        var currentUser = await userService.GetCurrentUserAsync();
        var profileUser = await userService.GetUserAsyncByUserName(username: username!);
        if (profileUser is null || currentUser is null)
        {
            return NotFound();
        }

        var publicProfile = new PublicProfileViewModel
        {
            UserId = profileUser.Id,
            UserName = profileUser.UserName!,
            Name = profileUser.Name!,
            Avatar = profileUser.Avatar!,
            Email = profileUser.ShowEmail ? profileUser.Email : null,
            Address = profileUser.Address,
            PhoneNumber = profileUser.PhoneNumber,
            RecentBlogs = await blogService.GetBlogsByUserId(userId: profileUser.Id, count: 6),
            Friends = await friendService.GetFriends(userId: profileUser.Id),
            IsFriend = friendService.AreFriends(userId: currentUser.Id, friendId: profileUser.Id),
            IsSendRequest = friendService.Find(userId: profileUser.Id, friendId: currentUser!.Id) != null,
            AllowChat = profileUser.AllowChat,
            AllowAddFriend = profileUser.AllowAddFriend,
            CurrentUser = currentUser,
            Website = profileUser.PersonalWebsite,
            Github = profileUser.Github is not null ? $"https://github.com/{profileUser.Github}" : null,
            Twitter = profileUser.Twitter is not null ? $"https://twitter.com/{profileUser.Twitter}" : null,
            Instagram = profileUser.Instagram is not null ? $"https://instagram.com/{profileUser.Instagram}" : null,
            Facebook = profileUser.Facebook is not null ? $"https://facebook.com/{profileUser.Facebook}" : null,

            // test
            RecentActivities = new RecentActivities
            {
                CommentPercentage = 80,
                LikePercentage = 72,
                SharePercentage = 89,
                RatingPercentage = 55,
                FollowPercentage = 66
            },
        };

        return View(model: publicProfile);
    }
}