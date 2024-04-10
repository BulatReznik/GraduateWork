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
        public string Login { get; init; } = null!;

        /// <summary>
        /// Ид. пользователя
        /// </summary>
        public string Id { get; init; } = null!;
    }
}
