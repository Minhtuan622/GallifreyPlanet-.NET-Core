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

        public async Task<List<CommentViewModel>> Get(
            CommentableType commentableType,
            int commentableId
        )
        {
            List<Comment>? comments = _context.Comment
                .Where(c =>
                    c.CommentableType == commentableType &&
                    c.CommentableId == commentableId &&
                    c.ParentId == null
                )
                .ToList();

            List<CommentViewModel>? result = new List<CommentViewModel>();

            foreach (Comment item in comments)
            {
                result.Add(await NewCommentViewModel(item));
            }

            return result;
        }

        public Comment? GetById(int id)
        {
            return _context.Comment.FirstOrDefault(c => c.Id == id);
        }

        public async Task<List<CommentViewModel>> GetReplies(
            CommentableType commentableType,
            int commentableId,
            int parentId
        )
        {
            List<Comment>? comments = _context.Comment
                .Where(c =>
                    c.CommentableType == commentableType &&
                    c.CommentableId == commentableId &&
                    c.ParentId == parentId
                )
                .ToList();

            List<CommentViewModel>? result = new List<CommentViewModel>();

            foreach (Comment item in comments)
            {
                result.Add(await NewCommentViewModel(item));
            }

            return result;
        }

        public bool DeleteCommentChildren(Comment comment)
        {
            try
            {
                List<Comment>? replies = _context.Comment
               .Where(c =>
                   c.CommentableType == comment.CommentableType &&
                   c.CommentableId == comment.CommentableId &&
                   c.ParentId == comment.Id
               )
               .ToList();

                foreach (Comment item in replies)
                {
                    _context.Comment.Remove(item);
                }

                _context.Comment.Remove(comment);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<CommentViewModel> NewCommentViewModel(Comment comment)
        {
            CommentViewModel? newComment = new CommentViewModel
            {
                Id = comment.Id,
                User = await _userService.GetUserAsyncById(comment.UserId!),
                ParentId = comment.ParentId,
                Replies = await GetReplies(comment.CommentableType, comment.CommentableId, comment.Id),
                CommentableId = comment.CommentableId,
                CommentableType = comment.CommentableType,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
            };

            return newComment;
        }
    }
}
