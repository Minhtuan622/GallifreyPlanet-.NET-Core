using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers;

public class CommentController(
    GallifreyPlanetContext context,
    UserService userService,
    CommentService commentService
)
    : Controller
{
    private JsonResult JsonResponse(bool success, string message = "", object? data = null)
    {
        return Json(data: new { success, message, data });
    }

    private async Task<User> GetAuthenticatedUserAsync()
    {
        var user = await userService.GetCurrentUserAsync();
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
        return string.IsNullOrWhiteSpace(value: content)
            ? JsonResponse(success: false, message: "Bình luận không được để trống")
            : null;
    }

    [HttpGet]
    public async Task<JsonResult> Get(int commentableId)
    {
        try
        {
            return JsonResponse(
                success: true,
                data: await commentService.Get(
                    commentableType: CommentableType.Blog,
                    commentableId: commentableId
                )
            );
        }
        catch (Exception ex)
        {
            return JsonResponse(success: false, message: ex.Message);
        }
    }

    [HttpDelete]
    public async Task<JsonResult> Delete(int id)
    {
        try
        {
            var comment = commentService.GetById(id: id);
            if (comment == null)
            {
                return JsonResponse(
                    success: false,
                    message: "Bình luận không được để trống"
                );
            }

            var user = await GetAuthenticatedUserAsync();
            if (user.Id != comment.UserId)
            {
                return JsonResponse(
                    success: false,
                    message: "Bạn không có quyền xóa bình luận này"
                );
            }

            var deleteSuccess = commentService.DeleteCommentChildren(comment: comment);

            TempData[key: "StatusMessage"] = "Xóa bình luận thành công";
            return JsonResponse(
                success: deleteSuccess,
                message: deleteSuccess ? "Xóa bình luận thành công" : "Xóa bình luận không thành công"
            );
        }
        catch (Exception ex)
        {
            return JsonResponse(success: false, message: ex.Message);
        }
    }

    [HttpPost]
    public async Task<JsonResult> AddReply(int commentId, string content)
    {
        try
        {
            var contentValidation = ValidateContent(content: content);
            if (contentValidation != null)
            {
                return contentValidation;
            }

            var user = await GetAuthenticatedUserAsync();
            var parentComment = commentService.GetById(id: commentId);
            if (parentComment == null)
            {
                return JsonResponse(
                    success: false,
                    message: "Bình luận bạn vừa phản hồi không còn tồn tại"
                );
            }

            var reply = new Comment
            {
                ParentId = commentId,
                CommentableType = CommentableType.Blog,
                CommentableId = parentComment.CommentableId,
                UserId = user.Id,
                Content = content.Trim(),
                CreatedAt = DateTime.Now
            };

            await context.Comment.AddAsync(entity: reply);
            await context.SaveChangesAsync();

            return JsonResponse(success: true, message: "Phản hồi thành công");
        }
        catch (Exception ex)
        {
            return JsonResponse(success: false, message: ex.Message);
        }
    }

    [HttpDelete]
    public async Task<JsonResult> DeleteReply(int id)
    {
        try
        {
            var reply = commentService.GetById(id: id);
            if (reply == null)
            {
                return JsonResponse(
                    success: false,
                    message: "Bình luận không tồn tại"
                );
            }

            var user = await GetAuthenticatedUserAsync();
            if (user.Id != reply.UserId)
            {
                return JsonResponse(
                    success: false,
                    message: "Bạn không có quyền xóa bình luận này"
                );
            }

            context.Comment.Remove(entity: reply);
            await context.SaveChangesAsync();

            TempData[key: "StatusMessage"] = "Xóa bình luận thành công";
            return JsonResponse(success: true, message: "Xóa bình luận thành công");
        }
        catch (Exception ex)
        {
            return JsonResponse(success: false, message: ex.Message);
        }
    }
}