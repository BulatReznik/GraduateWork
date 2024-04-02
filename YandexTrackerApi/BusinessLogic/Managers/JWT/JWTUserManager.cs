using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using YandexTrackerApi.BusinessLogic.Models.Enums;
using YandexTrackerApi.BusinessLogic.Models.UserModels;

namespace YandexTrackerApi.BusinessLogic.Managers.JWT
{
    public class JWTUserManager : IJWTUserManager
    {
        private readonly IJWTAuthManager _jwtAuthManager;
        private readonly ISecurityTokenValidator _securityTokenValidator;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public JWTUserManager(
            IJWTAuthManager jwtAuthManager,
            ISecurityTokenValidator securityTokenValidator,
            ILogger logger,
            IMediator mediator)
        {
            _jwtAuthManager = jwtAuthManager;
            _securityTokenValidator = securityTokenValidator;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<UserResponseData> GetUserByIdentity(ClaimsIdentity identity)
        {
            if (!Guid.TryParse(identity?.FindFirst("Id")?.Value, out Guid id))
            {
                throw new Exception("Не удалось получить Id пользователя");
            };

            var result = await _mediator.Send(new UserDataQuery
            {
                Id = id,
            }) ?? throw new Exception("Пользователь не был найден");

            return result;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _jwtAuthManager.GetSymmetricSecurityKey(JwtTokenCommandType.Refresh),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true // Важно! Мы не проверяем срок действия токена при обновлении
                };

                var principal = _securityTokenValidator.ValidateToken(
                    token,
                    tokenValidationParameters,
                    out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken)
                    throw new InvalidOperationException("Ошибка получения SecurityToken");

                var jwtTokenValid = jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);

                if (!jwtTokenValid)
                    throw new SecurityTokenValidationException("Ошибка валидации SecurityToken");

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения пользователя по истекшему токену");
                throw;
            }
        }
    }
}
