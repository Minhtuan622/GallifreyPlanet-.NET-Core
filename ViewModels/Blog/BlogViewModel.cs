using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.Blog
{
    public class BlogViewModel
    {
        public int Id { get; set; }
        public string? UserId { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        public string? Title { get; set; }

        [Display(Name = "Mô tả")]
        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        public string? Description { get; set; }

        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string? Content { get; set; }

        [Display(Name = "Ảnh thu nhỏ")]
        public IFormFile? ThumbnailFile { get; set; }

        public string? CurrentThumbnailPath { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
