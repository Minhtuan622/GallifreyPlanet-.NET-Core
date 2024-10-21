using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Friend;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class FriendController : Controller
    {
        private readonly UserService _userService;
        private readonly FriendService _friendService;

        public FriendController(
            UserService userService,
            FriendService friendService
        )
        {
            _userService = userService;
            _friendService = friendService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (user is null)
            {
                return NotFound();
            }

            string userId = user.Id;
            FriendManagerViewModel? friends = new FriendManagerViewModel
            {
                Friends = await _friendService.GetFriends(userId),
                FriendRequests = await _friendService.GetFriendRequests(userId),
            };

            return View(friends);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(string? friendId)
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (_friendService.SendFriendRequest(user.Id, friendId))
            {
                TempData[key: "StatusMessage"] = "Gửi lời mời kết bạn thành công";
                return RedirectToAction(nameof(Index), controllerName: "PublicProfile", user.UserName);
            }

            TempData[key: "StatusMessage"] = "Error while sending friend request";
            return RedirectToAction(nameof(Index), controllerName: "PublicProfile", user.UserName);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(string? friendId)
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (_friendService.Accept(user.Id, friendId))
            {
                TempData[key: "StatusMessage"] = "Đã chấp nhận lời mời kết bạn";
                return RedirectToAction(nameof(Index));
            }

            TempData[key: "StatusMessage"] = "Error while accept friend request";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeclineFriendRequest(string? friendId)
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (_friendService.Decline(user.Id, friendId))
            {
                TempData[key: "StatusMessage"] = "Đã từ chối lời mời kết bạn";
                return RedirectToAction(nameof(Index));
            }

            TempData[key: "StatusMessage"] = "Error while decline friend request";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(string? friendId)
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (_friendService.Remove(user.Id, friendId))
            {
                TempData[key: "StatusMessage"] = "Đã hủy kết bạn thành công";
                return RedirectToAction(nameof(Index));
            }

            TempData[key: "StatusMessage"] = "Error while remove friend";
            return RedirectToAction(nameof(Index));
        }
    }
}
