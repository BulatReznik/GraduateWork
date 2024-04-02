using MediatR;
using System.Security.Claims;
using YandexTrackerApi.BusinessLogic.Managers.JWT;
using YandexTrackerApi.BusinessLogic.Models.Enums;
using YandexTrackerApi.BusinessLogic.Models.TokenModels;
using YandexTrackerApi.BusinessLogic.Models;

namespace YandexTrackerApi.BusinessLogic.Commands.TokenCommands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResponseModel<RefreshTokenResponseDTO>>
    {
        private readonly ILogger _logger;
        private readonly IJWTUserManager _jwtUserManager;
        private readonly IJWTAuthManager _jwtAuthManager;

        public RefreshTokenCommandHandler(ILogger logger, IJWTAuthManager jwtAuthManager, IJWTUserManager jwtUserManager)
        {
            _logger = logger;
            _jwtUserManager = jwtUserManager;
            _jwtAuthManager = jwtAuthManager;
        }

        public Task<ResponseModel<RefreshTokenResponseDTO>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Проверка валидности refresh токена
                var principal = _jwtUserManager.GetPrincipalFromExpiredToken(command.TokenHash);

                if (principal == null)
                    return Task.FromResult(new ResponseModel<RefreshTokenResponseDTO> { ErrorMessage = "Неверный refresh токен." });

                if (principal.Identity is not ClaimsIdentity identity)
                    return Task.FromResult(new ResponseModel<RefreshTokenResponseDTO> { ErrorMessage = "Ошибка получения сущности пользователя" });

                var accessToken = _jwtAuthManager.GenerateToken(identity.Claims, JwtTokenCommandType.Access);

                return Task.FromResult(new ResponseModel<RefreshTokenResponseDTO>
                {
                    Data = new RefreshTokenResponseDTO
                    {
                        AccessToken = accessToken,
                        Id = identity.Claims.Last().Value,
                        Login = identity.Name,
                    }
                });
            }
            catch (Exception ex)
            {
                var errorMessage = "Ошибка обновления токена";
                _logger.LogError(ex, errorMessage);
                return Task.FromResult(new ResponseModel<RefreshTokenResponseDTO> { ErrorMessage = errorMessage });
            }
        }
    }
}
