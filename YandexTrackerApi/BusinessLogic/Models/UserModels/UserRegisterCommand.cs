using MediatR;
using System.ComponentModel.DataAnnotations;

namespace YandexTrackerApi.BusinessLogic.Models.UserModels
{
    public class UserRegisterCommand : IRequest<ResponseModel<UserRegisterResponseDTO>>
    {
        [Required(ErrorMessage = "Поле Name должно быть заполнено")]
        [StringLength(255, ErrorMessage = "Максимальная длина имени - 255 символов")]
        public string Name { get; init; } = null!;

        [Required(ErrorMessage = "Поле Login должно быть заполнено")]
        [StringLength(255, ErrorMessage = "Максимальная длина Login - 255 символов")]
        public string Login { get; init; } = null!;

        [Required(ErrorMessage = "Поле Password должно быть заполнено")]
        [StringLength(255, ErrorMessage = "Максимальная длина Password - 255 символов")]
        public string Password { get; init; } = null!;
    }
}
