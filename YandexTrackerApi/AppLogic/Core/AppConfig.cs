namespace YandexTrackerApi.AppLogic.Core
{
    /// <summary>
    /// Конфигурация приложения
    /// appsettings.json
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        public string DBConnectionString { get; set; } = null!;

        /// <summary>
        /// Издатель токена
        /// </summary>
        public string JWTIssuer { get; set; } = null!;

        /// <summary>
        /// Потребитель токена
        /// </summary>
        public string JWTAudience { get; set; } = null!;

        /// <summary>
        /// Ключ для шифрации
        /// </summary>
        public string JWTAccessKey { get; set; } = null!;

        /// <summary>
        /// Ключ для обновления
        /// </summary>
        public string JWTRefreshKey { get; set; } = null!;
    }
}
