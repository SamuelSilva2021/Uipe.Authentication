using System.ComponentModel.DataAnnotations;

namespace Authentication.Views
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
