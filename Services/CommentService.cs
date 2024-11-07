using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Comment;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Services;

public class CommentService(
    GallifreyPlanetContext context,
    UserService userService,
    NotificationService notificationService
)
{
    private async Task<List<CommentViewModel>> FetchCommentsAsync(
        CommentableType commentableType,
        int commentableId,
        int? parentId = null
    )
    {
        var comments = await context.Comment
            .Where(predicate: c =>
                c.CommentableType == commentableType &&
                c.CommentableId == commentableId &&
                c.ParentId == parentId
            )
            .OrderByDescending(keySelector: c => c.CreatedAt)
            .ToListAsync();

        var result = new List<CommentViewModel>();
        foreach (var comment in comments)
        {
            result.Add(item: await NewCommentViewModel(comment: comment));
        }

        return result;
    }

    public Task<List<CommentViewModel>> Get(CommentableType commentableType, int commentableId)
    {
        return FetchCommentsAsync(commentableType: commentableType, commentableId: commentableId);
    }

    public Comment? GetById(int id)
    {
        return context.Comment.FirstOrDefault(predicate: c => c.Id == id);
    }

    public Task<List<CommentViewModel>> GetReplies(CommentableType commentableType, int commentableId, int parentId)
    {
        return FetchCommentsAsync(commentableType: commentableType, commentableId: commentableId, parentId: parentId);
    }

    public async Task<CommentViewModel?> Add(int commentableId, string content)
    {
        try
        {
            var user = await userService.GetCurrentUserAsync();
            if (user is null || string.IsNullOrWhiteSpace(value: content))
            {
                return null;
            }

            var comment = new Comment
            {
                UserId = user.Id,
                CommentableId = commentableId,
                CommentableType = CommentableType.Blog,
                Content = content.Trim(),
                CreatedAt = DateTime.Now
            };

            await context.Comment.AddAsync(entity: comment);
            await context.SaveChangesAsync();

            await notificationService.CreateNotification(
                userId: user.Id,
                message: "Bài viết của bạn có bình luận mới.",
                type: NotificationType.Comment
            );

            return await NewCommentViewModel(comment: comment);
        }
        catch (Exception e)
        {
            Console.WriteLine(value: e);
            return null;
        }
    }

    public bool DeleteCommentChildren(Comment comment)
    {
        try
        {
            var replies = context.Comment
                .Where(predicate: c =>
                    c.CommentableType == comment.CommentableType &&
                    c.CommentableId == comment.CommentableId &&
                    c.ParentId == comment.Id
                )
                .ToList();

            context.Comment.RemoveRange(entities: replies);
            context.Comment.Remove(entity: comment);
            context.SaveChanges();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<CommentViewModel> NewCommentViewModel(Comment comment)
    {
        return new CommentViewModel
        {
            Id = comment.Id,
            User = await userService.GetUserAsyncById(userId: comment.UserId!),
            ParentId = comment.ParentId,
            Replies = await FetchCommentsAsync(commentableType: comment.CommentableType,
                commentableId: comment.CommentableId, parentId: comment.Id),
            CommentableId = comment.CommentableId,
            CommentableType = comment.CommentableType,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
        };
    }
}