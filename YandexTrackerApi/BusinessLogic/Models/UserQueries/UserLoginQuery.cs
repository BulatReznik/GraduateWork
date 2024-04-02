using MediatR;
using System.ComponentModel.DataAnnotations;
using YandexTrackerApi.BusinessLogic.Models.UserModels;

namespace YandexTrackerApi.BusinessLogic.Models.UserQueries
{
    /// <summary>
    /// Команда на логин
    /// </summary>
    public record UserLoginQuery : IRequest<ResponseModel<UserLoginResponseModel>>
    {
        /// <summary>
        /// Логин
        /// </summary>
        [Required(ErrorMessage = "поле Login не может быть пустым")]
        public string Login { get; init; } = null!;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "поле Password не может быть пустым")]
        public string Password { get; init; } = null!;
    }
}
