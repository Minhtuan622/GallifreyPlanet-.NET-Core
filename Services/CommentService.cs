using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Comment;

namespace GallifreyPlanet.Services
{
    public class CommentService
    {
        private readonly GallifreyPlanetContext _context;
        private readonly BlogService _blogService;
        private readonly UserService _userService;

        public CommentService(
            GallifreyPlanetContext context,
            BlogService blogService,
            UserService userService
        )
        {
            _context = context;
            _blogService = blogService;
            _userService = userService;
        }

        public async Task<List<CommentViewModel>> GetComments(
            string userId,
            CommentableType commentableType,
            int commentableId
        )
        {
            List<Comment>? comments = _context.Comment
                .Where(c =>
                    c.UserId == userId &&
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

        public async Task<CommentViewModel> NewCommentViewModel(Comment comment)
        {
            return new CommentViewModel
            {
                Id = comment.Id,
                User = await _userService.GetUserAsyncById(comment.UserId!),
                Blog = _blogService.GetById(comment.CommentableId),
                CommentableId = comment.CommentableId,
                CommentableType = comment.CommentableType,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
            };
        }
    }
}
