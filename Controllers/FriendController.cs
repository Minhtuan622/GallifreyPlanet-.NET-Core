﻿using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Friend;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class FriendController(
        UserService userService,
        FriendService friendService)
        : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User? user = await userService.GetCurrentUserAsync();
            if (user is null)
            {
                return NotFound();
            }

            string userId = user.Id;
            FriendManagerViewModel friends = new FriendManagerViewModel
            {
                Friends = await friendService.GetFriends(userId: userId),
                FriendRequests = await friendService.GetFriendRequests(userId: userId),
                BlockedFriends = await friendService.GetBlockedFriends(userId: userId),
            };

            return View(model: friends);
        }

        [HttpPost]
        public async Task<IActionResult> Send(string? friendId)
        {
            User? user = await userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (friendService.Send(userId: user.Id, friendId: friendId))
            {
                TempData[key: "StatusMessage"] = "Gửi lời mời kết bạn thành công";
                return RedirectToAction(actionName: nameof(Index));
            }

            TempData[key: "StatusMessage"] = "Error while sending friend request";
            return RedirectToAction(actionName: nameof(Index), controllerName: "PublicProfile", routeValues: new { username = user.UserName });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(string? friendId)
        {
            User? user = await userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (friendService.Cancel(userId: user.Id, friendId: friendId))
            {
                TempData[key: "StatusMessage"] = "Hủy lời mời kết bạn thành công";
                return RedirectToAction(actionName: nameof(Index));
            }

            TempData[key: "StatusMessage"] = "Error while canceling friend request";
            return RedirectToAction(actionName: nameof(Index), controllerName: "PublicProfile", routeValues: new { username = user.UserName });
        }

        [HttpPost]
        public async Task<IActionResult> Accept(string? friendId)
        {
            User? user = await userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (friendService.Accept(userId: user.Id, friendId: friendId))
            {
                TempData[key: "StatusMessage"] = "Đã chấp nhận lời mời kết bạn";
                return RedirectToAction(actionName: nameof(Index));
            }

            TempData[key: "StatusMessage"] = "Error while accept friend request";
            return RedirectToAction(actionName: nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Decline(string? friendId)
        {
            User? user = await userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (friendService.Decline(userId: user.Id, friendId: friendId))
            {
                TempData[key: "StatusMessage"] = "Đã từ chối lời mời kết bạn";
                return RedirectToAction(actionName: nameof(Index));
            }

            TempData[key: "StatusMessage"] = "Error while decline friend request";
            return RedirectToAction(actionName: nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Blocked(string? friendId)
        {
            User? user = await userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (friendService.Blocked(userId: user.Id, friendId: friendId))
            {
                TempData[key: "StatusMessage"] = "Đã chặn thành công";
                return RedirectToAction(actionName: nameof(Index));
            }

            TempData[key: "StatusMessage"] = "Error while block friend";
            return RedirectToAction(actionName: nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UnBlocked(string? friendId)
        {
            User? user = await userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (friendService.UnBlocked(userId: user.Id, friendId: friendId))
            {
                TempData[key: "StatusMessage"] = "Bỏ chặn thành công";
                return RedirectToAction(actionName: nameof(Index));
            }

            TempData[key: "StatusMessage"] = "Error while unblock friend";
            return RedirectToAction(actionName: nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(string? friendId)
        {
            User? user = await userService.GetCurrentUserAsync();
            if (friendId is null || user is null || friendId == user.Id)
            {
                return NotFound();
            }

            if (friendService.Remove(userId: user.Id, friendId: friendId))
            {
                TempData[key: "StatusMessage"] = "Đã hủy kết bạn thành công";
                return RedirectToAction(actionName: nameof(Index));
            }

            TempData[key: "StatusMessage"] = "Error while remove friend";
            return RedirectToAction(actionName: nameof(Index));
        }
    }
}
