using System.ComponentModel.DataAnnotations;

namespace Authentication.Views
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [StringLength(50, ErrorMessage = "Email deve ter no máximo 50 caractéres")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Email deve ter no máximo 50 caractéres")]
        public string Password { get; set; }
    }
}
