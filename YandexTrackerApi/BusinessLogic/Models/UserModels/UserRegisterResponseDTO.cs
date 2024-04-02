namespace YandexTrackerApi.BusinessLogic.Models.UserModels
{
    public class UserRegisterResponseDTO
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
        public string? Login { get; init; }

        /// <summary>
        /// Ид. пользователя
        /// </summary>
        public string Id { get; init; } = null!;
    }
}
