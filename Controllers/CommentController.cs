using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Controllers
{
    public class CommentController : Controller
    {
        private readonly GallifreyPlanetContext _context;
        private readonly UserService _userService;
        private readonly BlogService _blogService;

        public CommentController(
            GallifreyPlanetContext context,
            UserService userService,
            BlogService blogService
        )
        {
            _context = context;
            _userService = userService;
            _blogService = blogService;
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(int blogId, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return BadRequest(error: "Nội dung bình luận không được để trống");
                }

                if (!_blogService.BlogExists(blogId))
                {
                    return NotFound(value: "Không tìm thấy bài viết");
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null)
                {
                    return Unauthorized(value: "Người dùng không tồn tại");
                }

                Comment? comment = new Comment
                {
                    UserId = user.Id,
                    CommentableId = blogId,
                    CommentableType = CommentableType.blog,
                    Content = content.Trim(),
                    CreatedAt = DateTime.Now
                };

                await _context.Comment.AddAsync(comment);
                await _context.SaveChangesAsync();

                // Nếu là Ajax request thì trả về partial view
                if (Request.Headers[key: "X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView(viewName: "_CommentPartial", comment);
                }

                // Nếu không phải Ajax thì redirect về trang blog
                return RedirectToAction(actionName: "Details", controllerName: "Blogs", new { id = blogId });
            }
            catch (Exception)
            {
                return StatusCode(statusCode: 500, value: "Đã xảy ra lỗi khi thêm bình luận");
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                Comment? comment = await _context.Comment
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (comment == null)
                {
                    return NotFound(value: "Không tìm thấy bình luận");
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null || user.Id != comment.UserId)
                {
                    return Unauthorized(value: "Bạn không có quyền xóa bình luận này");
                }

                // Xóa các reply trước
                if (comment.Replies?.Any() == true)
                {
                    _context.Reply.RemoveRange(comment.Replies);
                }

                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(statusCode: 500, value: "Đã xảy ra lỗi khi xóa bình luận");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReply(int commentId, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return BadRequest(error: "Nội dung phản hồi không được để trống");
                }

                // Kiểm tra comment có tồn tại
                Comment? comment = await _context.Comment.FindAsync(commentId);
                if (comment == null)
                {
                    return NotFound(value: "Không tìm thấy bình luận");
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null)
                {
                    return Unauthorized();
                }

                Reply? reply = new Reply
                {
                    ParentId = commentId,
                    UserId = user.Id,
                    Content = content.Trim(),
                    CreatedAt = DateTime.Now
                };

                await _context.Reply.AddAsync(reply);
                await _context.SaveChangesAsync();

                // Nếu là Ajax request thì trả về partial view
                if (Request.Headers[key: "X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView(viewName: "_ReplyPartial", reply);
                }

                // Nếu không phải Ajax thì redirect về trang blog
                return RedirectToAction(actionName: "Details", new { id = comment.CommentableId });
            }
            catch (Exception)
            {
                return StatusCode(statusCode: 500, value: "Đã xảy ra lỗi khi thêm phản hồi");
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteReply(int id)
        {
            try
            {
                Reply? reply = await _context.Reply.FindAsync(id);
                if (reply == null)
                {
                    return NotFound(value: "Không tìm thấy phản hồi");
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null || user.Id != reply.UserId)
                {
                    return Unauthorized(value: "Bạn không có quyền xóa phản hồi này");
                }

                _context.Reply.Remove(reply);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(statusCode: 500, value: "Đã xảy ra lỗi khi xóa phản hồi");
            }
        }
    }
}
