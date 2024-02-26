using System.ComponentModel.DataAnnotations;

namespace Authentication.Views
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [EmailAddress] 
        public string Email { get; set;}
        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string NewPassword { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 4)]
        [Compare(nameof(NewPassword), ErrorMessage = "Senhas não conferem")]
        public string ConfirmPassword { get; set; }
    }
}
