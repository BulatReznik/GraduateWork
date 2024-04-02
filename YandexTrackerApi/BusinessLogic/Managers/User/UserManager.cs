using MediatR;
using System.Security.Authentication;
using System.Security.Claims;
using YandexTrackerApi.BusinessLogic.Models.UserModels;

namespace YandexTrackerApi.BusinessLogic.Managers.User
{
    public class UserManager : IUserManager
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        public UserManager(
        IMediator mediator,
        ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public Guid GetCurrentUserIdByContext(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var identity = httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity
                    ?? throw new Exception("Ошибка получения ClaimsIdentity");

                if (!identity.IsAuthenticated)
                    throw new AuthenticationException("Пользователь не вошёл в систему");

                var id = identity.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(id))
                    throw new AuthenticationException("Невозможно определить Id пользователя в контексте сервера");

                return Guid.Parse(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка определения Id пользователя");
                throw;
            }
        }

        public async Task<UserResponseData> GetCurrentUserDataAsync(
            IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var userId = GetCurrentUserIdByContext(httpContextAccessor);

                var userData = await _mediator.Send(new UserDataQuery
                {
                    Id = userId,
                });

                return userData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения данных о пользователе");
                throw;
            }
        }
    }
}
