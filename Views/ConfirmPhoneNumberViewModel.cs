using System.ComponentModel.DataAnnotations;

namespace Authentication.Views
{
    public class ConfirmPhoneNumberViewModel
    {
            [Phone(ErrorMessage = "Por favor preencha um número válido")]
            [Required(ErrorMessage = "Entre com número de telefone")]
            [Display(Name = "Número de celular")]
            public string PhoneNumber { get; set; }
    }
}
