using MediatR;
using System.Security.Claims;
using YandexTrackerApi.BusinessLogic.Models.UserModels;
using Microsoft.Extensions.Logging;

namespace YandexTrackerApi.BusinessLogic.Queries.UserQueries
{
    /// <summary>
    /// Получение сущности пользователя ClaimsIdentity
    /// по логину и паролю
    /// </summary>
    public class UserIdentityQueryHandler : IRequestHandler<UserIdentityQuery, ClaimsIdentity>
    {
        private readonly ILogger _logger;

        public UserIdentityQueryHandler(
            ILogger logger)
        {
            _logger = logger;
        }

        public async Task<ClaimsIdentity> Handle(
            UserIdentityQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                if (query.UserId == Guid.Empty || query.Login == null)
                {
                    var errrorMessage = $"Ошибка получения сущности ClaimsIdentity для пользователя {query.Login}";
                    _logger.LogError(errrorMessage);
                    throw new Exception(errrorMessage);
                }

                var claims = new List<Claim>
                {
                    new(ClaimsIdentity.DefaultNameClaimType, query.Login),
                    new("Id", query.UserId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return await Task.FromResult(claimsIdentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка получения сущности ClaimsIdentity для пользователя {query.Login}");
                throw;
            }
        }
    }
}
