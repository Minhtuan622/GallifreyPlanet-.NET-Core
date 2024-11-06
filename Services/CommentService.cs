using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Comment;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Services;

public class CommentService(GallifreyPlanetContext context, UserService userService)
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
            result.Add(item: await CreateCommentViewModelAsync(comment: comment));
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

    private async Task<CommentViewModel> CreateCommentViewModelAsync(Comment comment)
    {
        return new CommentViewModel
        {
            Id = comment.Id,
            User = await userService.GetUserAsyncById(userId: comment.UserId!),
            ParentId = comment.ParentId,
            Replies = await FetchCommentsAsync(commentableType: comment.CommentableType, commentableId: comment.CommentableId, parentId: comment.Id),
            CommentableId = comment.CommentableId,
            CommentableType = comment.CommentableType,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
        };
    }
}