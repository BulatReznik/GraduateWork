using System.Security.Claims;
using YandexTrackerApi.BusinessLogic.Models.UserModels;

namespace YandexTrackerApi.BusinessLogic.Managers.JWT
{
    public interface IJWTUserManager
    {
        Task<UserResponseData> GetUserByIdentity(ClaimsIdentity identity);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
