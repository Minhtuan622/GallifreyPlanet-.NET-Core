using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Comment;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Services
{
    public class CommentService
    {
        private readonly GallifreyPlanetContext _context;
        private readonly UserService _userService;

        public CommentService(GallifreyPlanetContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        private async Task<List<CommentViewModel>> FetchCommentsAsync(
            CommentableType commentableType,
            int commentableId,
            int? parentId = null
        )
        {
            List<Comment> comments = await _context.Comment
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
            return _context.Comment.FirstOrDefault(c => c.Id == id);
        }

        public Task<List<CommentViewModel>> GetReplies(CommentableType commentableType, int commentableId, int parentId)
        {
            return FetchCommentsAsync(commentableType, commentableId, parentId);
        }

        public bool DeleteCommentChildren(Comment comment)
        {
            try
            {
                List<Comment> replies = _context.Comment
                    .Where(c =>
                        c.CommentableType == comment.CommentableType &&
                        c.CommentableId == comment.CommentableId &&
                        c.ParentId == comment.Id
                    )
                    .ToList();

                _context.Comment.RemoveRange(replies);
                _context.Comment.Remove(comment);
                _context.SaveChanges();

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
                User = await _userService.GetUserAsyncById(comment.UserId!),
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
