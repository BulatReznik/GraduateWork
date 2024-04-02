using MediatR;
using YandexTrackerApi.BusinessLogic.Models.UserModels;

namespace YandexTrackerApi.BusinessLogic.Models.UserQueries
{
    /// <summary>
    /// Запрос данных о пользователе
    /// </summary>
    public record UserDataQuery : IRequest<UserResponseData>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? Login { get; init; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string? Password { get; init; }
    }
}
