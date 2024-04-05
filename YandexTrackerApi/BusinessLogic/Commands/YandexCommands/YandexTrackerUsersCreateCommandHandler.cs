using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using YandexTrackerApi.AppLogic.Core;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.YandexModels;
using YandexTrackerApi.DbModels;
using User = YandexTrackerApi.Models.YandexModels.User;

namespace YandexTrackerApi.BusinessLogic.Commands.YandexCommands
{
    public class YandexTrackerUsersCreateCommandHandler : IRequestHandler<YandexTrackerUsersCreateCommand, ResponseModel<string>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;
        private readonly AppConfig _appConfig;

        public YandexTrackerUsersCreateCommandHandler(ILogger logger,
            IGraduateWorkContext context,
            IOptions<AppConfig> options)
        {
            _logger = logger;
            _context = context;
            _appConfig = options.Value;
        }

        public async Task<ResponseModel<string>> Handle(YandexTrackerUsersCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем доступ пользователя к проекту
                var access = await CheckUserAccess(request.ProjectId, request.UserId);

                if (!access)
                    return new ResponseModel<string> { ErrorMessage = "У пользователя нет доступа к этому проекту" };

                // Получаем информацию о трекере
                var tracker = await GetTracker(request.ProjectId);

                if (tracker == null)
                    return new ResponseModel<string> { ErrorMessage = "Не удалось получить данные для доступа к яндекс трекеру" };

                // Получаем список пользователей из внешнего источника
                var externalUsers = await GetExternalUsers(tracker, cancellationToken);

                if (externalUsers?.Count > 0)
                {
                    // Маппим пользователей в объекты БД и добавляем их
                    var newUsersList = await MapExternalUsersToDbModels(externalUsers, request.ProjectId);

                    await AddNewUsersToDatabase(newUsersList);

                    return new ResponseModel<string> { Data = "Пользователи были успешно получены и занесены в базу данных" };
                }
                else
                    return new ResponseModel<string> { ErrorMessage = "Не удалось получить данные о пользователях из внешнего источника" };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось синхронизировать пользователей из яндекс трекера с пользователями системы";
                _logger.LogError(ex, errorMessage);
                return new ResponseModel<string> { ErrorMessage = errorMessage };
            }
        }

        /// <summary>
        /// Проверяет доступ пользователя к проекту
        /// </summary>
        private async Task<bool> CheckUserAccess(Guid projectId, Guid userId)
        {
            return await _context.UsersProjects.AnyAsync(up => up.ProjectId == projectId && up.UserId == userId);
        }

        /// <summary>
        /// Получает информацию о трекере
        /// </summary>
        private async Task<YandexTracker> GetTracker(Guid projectId)
        {
            var tracker = await _context.YandexTrackers.FirstOrDefaultAsync(yt => yt.Id == projectId);
            return tracker ?? throw new InvalidOperationException("Трекер не найден.");
        }

        /// <summary>
        /// Получает список пользователей из внешнего источника
        /// </summary>
        private async Task<List<User>> GetExternalUsers(YandexTracker tracker, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {tracker.OauthToken}");
            httpClient.DefaultRequestHeaders.Add("X-Cloud-Org-ID", tracker.CloudOrgId);

            var endPoint = "v2/users/";
            var response = await httpClient.GetAsync(_appConfig.YandexTrackerUrl + endPoint, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<User>>(responseBody) ?? throw new InvalidOperationException("Список пользователей не может быть пустым.");
            }
            else
            {
                var errorMessage = $"Не удалось получить данные. Код состояния: {response.StatusCode}";
                _logger.LogError(errorMessage);
                throw new HttpRequestException(errorMessage);
            }
        }


        /// <summary>
        /// Маппит пользователей из внешнего источника в объекты БД
        /// </summary>
        private async Task<List<YandexTrackerUser>> MapExternalUsersToDbModels(List<User> externalUsers, Guid projectId)
        {
            var newUsersList = new List<YandexTrackerUser>();

            foreach (var externalUser in externalUsers)
            {
                var existingUser = await _context.YandexTrackerUsers.FirstOrDefaultAsync(u => u.Login == externalUser.Login);
                if (existingUser != null)
                {
                    existingUser.Name = externalUser.FirstName + " " + externalUser.LastName;
                }
                else
                {
                    var newUser = new YandexTrackerUser
                    {
                        Id = externalUser.Uid.ToString(),
                        Login = externalUser.Login,
                        Name = externalUser.FirstName + " " + externalUser.LastName,
                        ProjectId = projectId
                    };
                    newUsersList.Add(newUser);
                }
            }
            return newUsersList;
        }

        /// <summary>
        /// Добавляет новых пользователей в базу данных
        /// </summary>
        private async Task AddNewUsersToDatabase(List<YandexTrackerUser> newUsersList)
        {
            await _context.YandexTrackerUsers.AddRangeAsync(newUsersList);
            await _context.SaveChangesAsync();
        }
    }
}
