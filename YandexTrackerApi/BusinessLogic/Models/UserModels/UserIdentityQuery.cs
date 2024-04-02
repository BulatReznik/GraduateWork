using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace YandexTrackerApi.BusinessLogic.Models.UserModels
{
    /// <summary>
    /// Создание сущности Identity пользователя
    /// по логину и паролю
    /// </summary>
    public record UserIdentityQuery : IRequest<ClaimsIdentity>
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле UserId должно быть заполнено")]
        public Guid UserId { get; init; }

        /// <summary>
        /// Логин
        /// </summary>
        [Required(ErrorMessage = "Поле Login должно быть заполнено")]
        public string Login { get; init; } = null!;

        /// <summary>
        /// Роль
        /// </summary>
        public string Role { get; init; } = null!;
    }
}
