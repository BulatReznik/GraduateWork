using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using YandexTrackerApi.BusinessLogic.Models.Enums;

namespace YandexTrackerApi.BusinessLogic.Managers.JWT
{
    public interface IJWTAuthManager
    {
        string GenerateToken(IEnumerable<Claim> claims, JwtTokenCommandType type);

        SymmetricSecurityKey GetSymmetricSecurityKey(JwtTokenCommandType type);
    }
}
