using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Friend;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers;

public class FriendController(
    UserService userService,
    FriendService friendService)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        var userId = user.Id;
        var friends = new FriendManagerViewModel
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
        var user = await userService.GetCurrentUserAsync();
        if (friendId is null || user is null || friendId == user.Id)
        {
            return NotFound();
        }

        if (friendService.Send(userId: user.Id, friendId: friendId))
        {
            TempData[key: "StatusMessage"] = "Gửi lời mời kết bạn thành công";
            TempData[key: "StatusType"] = "success";
            return RedirectToAction(actionName: nameof(Index));
        }

        TempData[key: "StatusMessage"] = "Có lỗi khi gửi lời mời kết bạn, vui lòng thử lại sau";
        TempData[key: "StatusType"] = "danger";
        return RedirectToAction(actionName: nameof(Index), controllerName: "PublicProfile", routeValues: new { username = user.UserName });
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(string? friendId)
    {
        var user = await userService.GetCurrentUserAsync();
        if (friendId is null || user is null || friendId == user.Id)
        {
            return NotFound();
        }

        if (friendService.Cancel(userId: user.Id, friendId: friendId))
        {
            TempData[key: "StatusMessage"] = "Hủy lời mời kết bạn thành công";
            TempData[key: "StatusType"] = "success";
            return RedirectToAction(actionName: nameof(Index));
        }

        TempData[key: "StatusMessage"] = "Có lỗi khi hủy lời mời kết bạn, vui lòng thử lại sau";
        TempData[key: "StatusType"] = "danger";
        return RedirectToAction(actionName: nameof(Index), controllerName: "PublicProfile", routeValues: new { username = user.UserName });
    }

    [HttpPost]
    public async Task<IActionResult> Accept(string? friendId)
    {
        var user = await userService.GetCurrentUserAsync();
        if (friendId is null || user is null || friendId == user.Id)
        {
            return NotFound();
        }

        if (friendService.Accept(userId: user.Id, friendId: friendId))
        {
            TempData[key: "StatusMessage"] = "Đã chấp nhận lời mời kết bạn";
            TempData[key: "StatusType"] = "success";
            return RedirectToAction(actionName: nameof(Index));
        }

        TempData[key: "StatusMessage"] = "Có lỗi khi chấp nhận lời mời kết bạn, vui lòng thử lại sau";
        TempData[key: "StatusType"] = "success";
        return RedirectToAction(actionName: nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Decline(string? friendId)
    {
        var user = await userService.GetCurrentUserAsync();
        if (friendId is null || user is null || friendId == user.Id)
        {
            return NotFound();
        }

        if (friendService.Decline(userId: user.Id, friendId: friendId))
        {
            TempData[key: "StatusMessage"] = "Đã từ chối lời mời kết bạn";
            TempData[key: "StatusType"] = "success";
            return RedirectToAction(actionName: nameof(Index));
        }

        TempData[key: "StatusMessage"] = "Có lỗi khi từ chối lời mời kết bạn, vui lòng thử lại sau";
        TempData[key: "StatusType"] = "danger";
        return RedirectToAction(actionName: nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Blocked(string? friendId)
    {
        var user = await userService.GetCurrentUserAsync();
        if (friendId is null || user is null || friendId == user.Id)
        {
            return NotFound();
        }

        if (friendService.Blocked(userId: user.Id, friendId: friendId))
        {
            TempData[key: "StatusMessage"] = "Đã chặn thành công";
            TempData[key: "StatusType"] = "success";
            return RedirectToAction(actionName: nameof(Index));
        }

        TempData[key: "StatusMessage"] = "Có lỗi khi chặn, vui lòng thử lại sau";
        TempData[key: "StatusType"] = "danger";
        return RedirectToAction(actionName: nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UnBlocked(string? friendId)
    {
        var user = await userService.GetCurrentUserAsync();
        if (friendId is null || user is null || friendId == user.Id)
        {
            return NotFound();
        }

        if (friendService.UnBlocked(userId: user.Id, friendId: friendId))
        {
            TempData[key: "StatusMessage"] = "Bỏ chặn thành công";
            TempData[key: "StatusType"] = "success";
            return RedirectToAction(actionName: nameof(Index));
        }

        TempData[key: "StatusMessage"] = "Có lỗi khi bỏ chặn, vui lòng thử lại sau";
        TempData[key: "StatusType"] = "danger";
        return RedirectToAction(actionName: nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(string? friendId)
    {
        var user = await userService.GetCurrentUserAsync();
        if (friendId is null || user is null || friendId == user.Id)
        {
            return NotFound();
        }

        if (friendService.Remove(userId: user.Id, friendId: friendId))
        {
            TempData[key: "StatusMessage"] = "Đã hủy kết bạn thành công";
            TempData[key: "StatusType"] = "success";
            return RedirectToAction(actionName: nameof(Index));
        }

        TempData[key: "StatusMessage"] = "Có lỗi khi hủy kết bạn, vui lòng thử lại sau";
        TempData[key: "StatusType"] = "danger";
        return RedirectToAction(actionName: nameof(Index));
    }
}