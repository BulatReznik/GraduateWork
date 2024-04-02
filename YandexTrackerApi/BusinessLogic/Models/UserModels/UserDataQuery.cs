using MediatR;

namespace YandexTrackerApi.BusinessLogic.Models.UserModels
{
    /// <summary>
    /// Запрос данных о пользователе
    /// </summary>
    public record UserDataQuery : IRequest<UserResponseData>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int? Id { get; init; }

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
