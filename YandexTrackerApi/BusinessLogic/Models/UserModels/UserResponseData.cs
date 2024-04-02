namespace YandexTrackerApi.BusinessLogic.Models.UserModels
{
    /// <summary>
    /// Модель ответа по запросу
    /// информации о пользователе
    /// </summary>
    public class UserResponseData
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; init; } = null!;

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; init; } = null!;
    }
}
