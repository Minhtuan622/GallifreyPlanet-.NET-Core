using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class CommentController(
        GallifreyPlanetContext context,
        UserService userService,
        CommentService commentService)
        : Controller
    {
        private JsonResult JsonResponse(bool success, string message = "", object? data = null)
        {
            return Json(new { success, message, data });
        }

        private async Task<User> GetAuthenticatedUserAsync()
        {
            User? user = await userService.GetCurrentUserAsync();
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
                User user = await GetAuthenticatedUserAsync();

                List<CommentViewModel> data = await commentService.Get(CommentableType.Blog, commentableId);
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

                User user = await GetAuthenticatedUserAsync();

                Comment comment = new Comment
                {
                    UserId = user.Id,
                    CommentableId = commentableId,
                    CommentableType = CommentableType.Blog,
                    Content = content.Trim(),
                    CreatedAt = DateTime.Now
                };

                await context.Comment.AddAsync(comment);
                await context.SaveChangesAsync();

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
                Comment? comment = commentService.GetById(id);
                if (comment == null)
                {
                    return JsonResponse(
                        success: false,
                        message: "Bình luận không được để trống"
                    );
                }

                User user = await GetAuthenticatedUserAsync();
                if (user.Id != comment.UserId)
                {
                    return JsonResponse(
                        success: false,
                        message: "Bạn không có quyền xóa bình luận này"
                    );
                }

                bool deleteSuccess = commentService.DeleteCommentChildren(comment);

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

                User user = await GetAuthenticatedUserAsync();

                Comment? parentComment = commentService.GetById(commentId);
                if (parentComment == null)
                {
                    return JsonResponse(
                        success: false,
                        message: "Bình luận bạn vừa phản hồi không còn tồn tại"
                    );
                }

                Comment reply = new Comment
                {
                    ParentId = commentId,
                    CommentableType = CommentableType.Blog,
                    CommentableId = parentComment.CommentableId,
                    UserId = user.Id,
                    Content = content.Trim(),
                    CreatedAt = DateTime.Now
                };

                await context.Comment.AddAsync(reply);
                await context.SaveChangesAsync();

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
                Comment? reply = commentService.GetById(id);
                if (reply == null)
                {
                    return JsonResponse(
                        success: false,
                        message: "Bình luận không tồn tại"
                    );
                }

                User user = await GetAuthenticatedUserAsync();
                if (user.Id != reply.UserId)
                {
                    return JsonResponse(
                        success: false,
                        message: "Bạn không có quyền xóa bình luận này"
                    );
                }

                context.Comment.Remove(reply);
                await context.SaveChangesAsync();

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
