using System.ComponentModel.DataAnnotations;

namespace Authentication.Views
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Número celular")]
        [Required(ErrorMessage = "Número de celular é obrigatório")]
        [RegularExpression("^([0-9]{11})$", ErrorMessage = "Numero de telefone inválido.")]
        public string PhoneNumber {  get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string ConfirmPassword { get; set; }

    }
}
