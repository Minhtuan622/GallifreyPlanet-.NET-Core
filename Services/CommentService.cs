using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Comment;

namespace GallifreyPlanet.Services
{
    public class CommentService
    {
        private readonly GallifreyPlanetContext _context;
        private readonly UserService _userService;

        public CommentService(
            GallifreyPlanetContext context,
            UserService userService
        )
        {
            _context = context;
            _userService = userService;
        }

        public async Task<List<CommentViewModel>> GetComments(
            CommentableType commentableType,
            int commentableId
        )
        {
            List<Comment>? comments = _context.Comment
                .Where(c =>
                    c.CommentableType == commentableType &&
                    c.CommentableId == commentableId
                )
                .ToList();

            List<CommentViewModel>? result = new List<CommentViewModel>();

            foreach (Comment item in comments)
            {
                result.Add(await NewCommentViewModel(item));
            }

            return result;
        }

        public async Task<List<CommentViewModel>?> GetReplies(
            CommentableType commentableType,
            int commentableId,
            int? parentId
        )
        {
            if (parentId is not null)
            {
                List<Comment>? replies = _context.Comment
                   .Where(c =>
                       c.CommentableType == commentableType &&
                       c.CommentableId == commentableId &&
                       c.ParentId == parentId
                   )
                   .ToList();

                List<CommentViewModel>? result = new List<CommentViewModel>();

                foreach (Comment item in replies)
                {
                    result.Add(await NewCommentViewModel(item, isParent: false));
                }

                return result;
            }
            return null;
        }

        public bool DeleteCommentChildren(
            CommentableType commentableType,
            int commentableId,
            int? parentId
        )
        {
            if (parentId is not null)
            {
                List<Comment>? replies = _context.Comment
               .Where(c =>
                   c.CommentableType == commentableType &&
                   c.CommentableId == commentableId &&
                   c.ParentId == parentId
               )
               .ToList();
                return true;
            }
            return false;
        }

        public async Task<CommentViewModel> NewCommentViewModel(Comment comment, bool isParent = true)
        {
            CommentViewModel? newComment = new CommentViewModel
            {
                Id = comment.Id,
                User = await _userService.GetUserAsyncById(comment.UserId!),
                CommentableId = comment.CommentableId,
                CommentableType = comment.CommentableType,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
            };

            if (isParent)
            {
                newComment.Replies = await GetReplies(comment.CommentableType, comment.CommentableId, comment.ParentId);
            }

            return newComment;
        }
    }
}
