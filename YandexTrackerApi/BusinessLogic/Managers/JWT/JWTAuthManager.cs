using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YandexTrackerApi.AppLogic.Core;
using YandexTrackerApi.BusinessLogic.Models.Enums;

namespace YandexTrackerApi.BusinessLogic.Managers.JWT
{
    public class JWTAuthManager : IJWTAuthManager
    {
        private const int LIFETIME_ACCESS = 600; // 10 минут
        private const int LIFETIME_REFRESH = 2_592_000; // 30 дней

        private readonly AppConfig _appConfig;

        public JWTAuthManager(IOptions<AppConfig> options)
        {
            _appConfig = options.Value;
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey(JwtTokenCommandType type) =>
            type switch
            {
                JwtTokenCommandType.Access => new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_appConfig.JWTAccessKey)),

                JwtTokenCommandType.Refresh => new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_appConfig.JWTRefreshKey)),

                _ => throw new NotImplementedException(type.ToString())
            };

        public string GenerateToken(IEnumerable<Claim> claims, JwtTokenCommandType type)
        {
            var now = DateTime.UtcNow;

            var lifetime = type switch
            {
                JwtTokenCommandType.Access => LIFETIME_ACCESS,
                JwtTokenCommandType.Refresh => LIFETIME_REFRESH,
                _ => throw new NotImplementedException(type.ToString()),
            };

            var signingCredentials = new SigningCredentials(
                GetSymmetricSecurityKey(type),
                SecurityAlgorithms.HmacSha256);

            var tokenConfig = new JwtSecurityToken(
                issuer: _appConfig.JWTIssuer,
                audience: _appConfig.JWTAudience,
                notBefore: now,
                claims: claims,
                expires: now.Add(TimeSpan.FromSeconds(lifetime)),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenConfig);
        }
    }
}
