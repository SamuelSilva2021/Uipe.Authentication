using System.ComponentModel.DataAnnotations;

namespace Authentication.Views
{
    public class CodeConfirmationNumberPhone
    {
        [Required(ErrorMessage = "Obrigatório código de verificação.")]
        [StringLength(6)]
        public string Code { get; set; }
    }
}
