using YandexTrackerApi.BusinessLogic.Models.UserModels;

namespace YandexTrackerApi.BusinessLogic.Managers.User
{
    public interface IUserManager
    {
        /// <summary>
        /// Получить данные о пользователе по контексту
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        Task<UserResponseData> GetCurrentUserDataAsync(IHttpContextAccessor httpContextAccessor);

        /// <summary>
        /// Определение Id залогиненного пользователя
        /// по контексту авторизации
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        int GetCurrentUserIdByContext(IHttpContextAccessor httpContextAccessor);
    }
}
