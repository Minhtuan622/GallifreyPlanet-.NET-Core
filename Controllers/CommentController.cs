using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class CommentController : Controller
    {
        private readonly GallifreyPlanetContext _context;
        private readonly UserService _userService;
        private readonly CommentService _commentService;

        public CommentController(
            GallifreyPlanetContext context,
            UserService userService,
            CommentService commentService
        )
        {
            _context = context;
            _userService = userService;
            _commentService = commentService;
        }

        private JsonResult JsonResponse(bool success, string message = "", object? data = null)
        {
            return Json(new { success, message, data });
        }

        private async Task<User> GetAuthenticatedUserAsync()
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                JsonResponse(
                    success: false,
                    message: "Vui lòng đăng nhập để sử dụng tính năng này"
                );
            }
            return user!;
        }

        private JsonResult? ValidateContent(string content)
        {
            return string.IsNullOrWhiteSpace(content)
                ? JsonResponse(success: false, message: "Bình luận không được để trống")
                : null;
        }

        [HttpGet]
        public async Task<JsonResult> Get(int commentableId)
        {
            try
            {
                User? user = await GetAuthenticatedUserAsync();

                List<CommentViewModel>? data = await _commentService.Get(CommentableType.blog, commentableId);
                return JsonResponse(success: true, data: data);
            }
            catch (Exception ex)
            {
                return JsonResponse(success: false, ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Add(int commentableId, string content)
        {
            try
            {
                JsonResult? contentValidation = ValidateContent(content);
                if (contentValidation != null)
                {
                    return contentValidation;
                }

                User? user = await GetAuthenticatedUserAsync();

                Comment? comment = new Comment
                {
                    UserId = user.Id,
                    CommentableId = commentableId,
                    CommentableType = CommentableType.blog,
                    Content = content.Trim(),
                    CreatedAt = DateTime.Now
                };

                await _context.Comment.AddAsync(comment);
                await _context.SaveChangesAsync();

                return JsonResponse(
                    success: true,
                    message: "Bình luận thành công"
                );
            }
            catch (Exception ex)
            {
                return JsonResponse(success: false, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                Comment? comment = _commentService.GetById(id);
                if (comment == null)
                {
                    return JsonResponse(
                        success: false,
                        message: "Bình luận không được để trống"
                    );
                }

                User? user = await GetAuthenticatedUserAsync();
                if (user.Id != comment.UserId)
                {
                    return JsonResponse(
                        success: false,
                        message: "Bạn không có quyền xóa bình luận này"
                    );
                }

                bool deleteSuccess = _commentService.DeleteCommentChildren(comment);

                TempData[key: "StatusMessage"] = "Xóa bình luận thành công";
                return JsonResponse(
                    deleteSuccess,
                    deleteSuccess ? "Xóa bình luận thành công" : "Xóa bình luận không thành công"
                );
            }
            catch (Exception ex)
            {
                return JsonResponse(success: false, ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddReply(int commentId, string content)
        {
            try
            {
                JsonResult? contentValidation = ValidateContent(content);
                if (contentValidation != null)
                {
                    return contentValidation;
                }

                User? user = await GetAuthenticatedUserAsync();

                Comment? parentComment = _commentService.GetById(commentId);
                if (parentComment == null)
                {
                    return JsonResponse(
                        success: false,
                        message: "Bình luận bạn vừa phản hồi không còn tồn tại"
                    );
                }

                Comment? reply = new Comment
                {
                    ParentId = commentId,
                    CommentableType = CommentableType.blog,
                    CommentableId = parentComment.CommentableId,
                    UserId = user.Id,
                    Content = content.Trim(),
                    CreatedAt = DateTime.Now
                };

                await _context.Comment.AddAsync(reply);
                await _context.SaveChangesAsync();

                return JsonResponse(success: true, message: "Phản hồi thành công");
            }
            catch (Exception ex)
            {
                return JsonResponse(success: false, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteReply(int id)
        {
            try
            {
                Comment? reply = _commentService.GetById(id);
                if (reply == null)
                {
                    return JsonResponse(
                        success: false,
                        message: "Bình luận không tồn tại"
                    );
                }

                User? user = await GetAuthenticatedUserAsync();
                if (user.Id != reply.UserId)
                {
                    return JsonResponse(
                        success: false,
                        message: "Bạn không có quyền xóa bình luận này"
                    );
                }

                _context.Comment.Remove(reply);
                await _context.SaveChangesAsync();

                TempData[key: "StatusMessage"] = "Xóa bình luận thành công";
                return JsonResponse(success: true, message: "Xóa bình luận thành công");
            }
            catch (Exception ex)
            {
                return JsonResponse(success: false, ex.Message);
            }
        }
    }
}
