using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.AccountManager
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [Display(Name = "Tên")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Avatar")]
        public IFormFile? AvatarFile { get; set; }

        public string? CurrentAvatar { get; set; }
    }
}