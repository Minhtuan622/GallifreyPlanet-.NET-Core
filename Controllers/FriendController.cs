using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class FriendController : Controller
    {
        private readonly GallifreyPlanetContext? _context;
        private readonly UserService _userService;
        private readonly FriendService _friendService;

        public FriendController(
            GallifreyPlanetContext? context,
            UserService userService,
            FriendService friendService
        )
        {
            _context = context;
            _userService = userService;
            _friendService = friendService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(string? receiverId)
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (receiverId is null || user is null)
            {
                return NotFound();
            }

            await _friendService.SendFriendRequest(user.Id, receiverId);

            TempData[key: "StatusMessage"] = "Gửi lời mời kết bạn thành công";
            return RedirectToAction(nameof(Index), controllerName: "PublicProfile");
        }

        [HttpPost]
        public IActionResult AcceptFriendRequest(int? friendRequestId)
        {
            if (friendRequestId is null)
            {
                return NotFound();
            }

            FriendRequest? request = _friendService.GetFriendRequestById(friendRequestId ?? 0);

            if (request is null)
            {
                return NotFound();
            }

            request!.Status = 1;
            _context!.SaveChanges();

            TempData[key: "StatusMessage"] = "Đã chấp nhận lời mời kết bạn";
            return View();
        }

        [HttpPost]
        public IActionResult DeclineFriendRequest(int? friendRequestId)
        {
            if (friendRequestId is null)
            {
                return NotFound();
            }

            FriendRequest? request = _friendService.GetFriendRequestById(friendRequestId ?? 0);

            if (request is null)
            {
                return NotFound();
            }

            request!.Status = 2;
            _context!.SaveChanges();

            TempData[key: "StatusMessage"] = "Đã từ chối lời mời kết bạn";
            return View();
        }
    }
}
