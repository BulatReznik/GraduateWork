using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BPMN.Models.Login
{
    public class LoginFormModel
    {
        [Required(ErrorMessage = "Поле Login должно быть заполнено")]
        [EmailAddress]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = "Поле Password должно быть заполнено")]
        [PasswordPropertyText]
        public string Password { get; set; } = null!;
    }
}
