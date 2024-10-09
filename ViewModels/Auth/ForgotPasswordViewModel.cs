using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.Auth
{
    public class ForgotPasswordViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc.")]
        public string? Email { get; set; }
    }
}
