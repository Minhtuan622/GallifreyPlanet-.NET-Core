using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Comment;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Services
{
    public class CommentService(GallifreyPlanetContext context, UserService userService)
    {
        private async Task<List<CommentViewModel>> FetchCommentsAsync(
            CommentableType commentableType,
            int commentableId,
            int? parentId = null
        )
        {
            List<Comment> comments = await context.Comment
                .Where(c =>
                    c.CommentableType == commentableType &&
                    c.CommentableId == commentableId &&
                    c.ParentId == parentId
                )
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            List<CommentViewModel> result = new List<CommentViewModel>();
            foreach (Comment comment in comments)
            {
                result.Add(await CreateCommentViewModelAsync(comment));
            }
            return result;
        }

        public Task<List<CommentViewModel>> Get(CommentableType commentableType, int commentableId)
        {
            return FetchCommentsAsync(commentableType, commentableId);
        }

        public Comment? GetById(int id)
        {
            return context.Comment.FirstOrDefault(c => c.Id == id);
        }

        public Task<List<CommentViewModel>> GetReplies(CommentableType commentableType, int commentableId, int parentId)
        {
            return FetchCommentsAsync(commentableType, commentableId, parentId);
        }

        public bool DeleteCommentChildren(Comment comment)
        {
            try
            {
                List<Comment> replies = context.Comment
                    .Where(c =>
                        c.CommentableType == comment.CommentableType &&
                        c.CommentableId == comment.CommentableId &&
                        c.ParentId == comment.Id
                    )
                    .ToList();

                context.Comment.RemoveRange(replies);
                context.Comment.Remove(comment);
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
                User = await userService.GetUserAsyncById(comment.UserId!),
                ParentId = comment.ParentId,
                Replies = await FetchCommentsAsync(comment.CommentableType, comment.CommentableId, comment.Id),
                CommentableId = comment.CommentableId,
                CommentableType = comment.CommentableType,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
            };
        }
    }
}
