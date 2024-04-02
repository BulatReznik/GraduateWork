using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models.UserModels;
using YandexTrackerApi.BusinessLogic.Models.UserQueries;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Queries.UserQueries
{
    public class UserDataQueryHandler : IRequestHandler<UserDataQuery, UserResponseData>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger _logger;

        public UserDataQueryHandler(
            IGraduateWorkContext context,
            ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserResponseData> Handle(UserDataQuery query, CancellationToken cancellationToken)
        {
            try
            {
                UserResponseData? result = null;

                // Фильтр по логину
                if (!string.IsNullOrEmpty(query.Login) && !string.IsNullOrEmpty(query.Password))
                {
                    var login = query.Login.ToLower();

                    result = await _context.Users
                           .Where(u => u.Login.ToLower() == login &&
                           u.Password == query.Password)
                           .Select(u => new UserResponseData
                           {
                               Id = u.Id,
                               Login = u.Login,
                               Name = u.Name,
                           })
                           .AsNoTracking()
                           .FirstOrDefaultAsync(cancellationToken);
                }

                // Фильтр по Id
                if (query.Id != Guid.Empty)
                {
                    result = await _context.Users
                           .Where(u => u.Id == query.Id)
                           .Select(u => new UserResponseData
                           {
                               Id = u.Id,
                               Login = u.Login,
                               Name = u.Name
                           })
                           .AsNoTracking()
                           .FirstOrDefaultAsync(cancellationToken);
                }

                return result ?? throw new Exception("Были введены неверные данные пользователя");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения пользователя по запросу");
                throw;
            }
        }
    }
}
