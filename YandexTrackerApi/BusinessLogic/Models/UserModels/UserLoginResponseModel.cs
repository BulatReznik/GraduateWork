namespace YandexTrackerApi.BusinessLogic.Models.UserModels
{
    /// <summary>
    /// Модель ответа на запрос
    /// </summary>
    public record UserLoginResponseModel
    {
        /// <summary>
        /// Токен доступа
        /// </summary>
        public string AccessToken { get; init; } = null!;

        /// <summary>
        /// Токен обновления?
        /// </summary>
        public string RefreshToken { get; init; } = null!;

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? Username { get; init; }

        /// <summary>
        /// Ид. пользователя
        /// </summary>
        public string Id { get; init; } = null!;
    }
}
