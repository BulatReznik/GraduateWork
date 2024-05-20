using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using System.Xml;
using YandexTrackerApi.AppLogic.Core;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.YandexModels;
using YandexTrackerApi.DbModels;
using YandexTrackerApi.Models.YandexModels;

namespace YandexTrackerApi.BusinessLogic.Commands.YandexCommands
{
    public class YandexTrackerIssueByUserByPeriodCommandHandler : IRequestHandler<YandexTrackerIssueByUserByPeriodCommand, ResponseModel<string>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger _logger;
        private readonly AppConfig _appConfig;

        public YandexTrackerIssueByUserByPeriodCommandHandler(IGraduateWorkContext context,
            ILogger logger,
            IOptions<AppConfig> options)
        {
            _context = context;
            _logger = logger;
            _appConfig = options.Value;
        }

        public async Task<ResponseModel<string>> Handle(YandexTrackerIssueByUserByPeriodCommand request, CancellationToken cancellationToken)
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

                var employee = await _context.YandexTrackerUsers.FirstOrDefaultAsync(ytu => ytu.Id == request.EmployeeId, cancellationToken);

                if (employee == null)
                    return new ResponseModel<string> { ErrorMessage = "Данного сотрудника нет в списке пользователей" };

                var externalIssues = await GetExternalIssue(tracker, employee.Login, request.StartDate, request.EndDate, cancellationToken);

                var issuesDbModels = await MapExternalIssuesToDbModels(externalIssues, employee.Id);

                await _context.YandexTrackerTasks.AddRangeAsync(issuesDbModels, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return new ResponseModel<string> { Data = "Задачи из таск-трекера записаны в бд" };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось синхронизировать задачи из яндекс трекера";
                _logger.LogError(ex, errorMessage);
                return new ResponseModel<string> { ErrorMessage = errorMessage };
            }
        }

        /// <summary>
        /// Перевод строки с временем в интовое значение
        /// </summary>
        /// <param name="timeString"></param>
        /// <returns></returns>
        public int ConvertStringToHour(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
            {
                return 0;
            }
            else
            {
                TimeSpan timeSpan = XmlConvert.ToTimeSpan(timeString);

                int days = (int)timeSpan.Days;
                int hours = (int)timeSpan.Hours;

                return (days * 8) + hours;
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
        /// Получаем список задач за период из внешнего источника
        /// </summary>
        private async Task<List<Issue>> GetExternalIssue(YandexTracker tracker, string assignee, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {tracker.OauthToken}");
            httpClient.DefaultRequestHeaders.Add("X-Cloud-Org-ID", tracker.CloudOrgId);

            var endPoint = "v2/issues/_search";

            var filter = new
            {
                query = $"Assignee: {assignee} Queue: BACKDEV \"Start Date\": \"{startDate:yyyy-MM-dd}\"..\"{endDate:yyyy-MM-dd}\""
            };

            var content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_appConfig.YandexTrackerUrl + endPoint, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(responseBody);
                return JsonConvert.DeserializeObject<List<Issue>>(responseBody) ?? throw new InvalidOperationException("Список задач не может быть пустым.");
            }
            else
            {
                var errorMessage = $"Не удалось получить данные. Код состояния: {response.StatusCode}";
                _logger.LogError(errorMessage);
                throw new HttpRequestException(errorMessage);
            }

        }

        // Маппит задачи из внешнего источника в объекты БД
        private async Task<List<YandexTrackerTask>> MapExternalIssuesToDbModels(List<Issue> externalIssues, string employeeId)
        {
            var issuesDbModels = new List<YandexTrackerTask>();

            foreach (var externalIssue in externalIssues)
            {
                var originalEstimation = ConvertStringToHour(externalIssue.OriginalEstimation);
                var spentTime = ConvertStringToHour(externalIssue.Spent);

                var existingTask = await _context.YandexTrackerTasks
                    .FirstOrDefaultAsync(ytt => ytt.Id == externalIssue.Id);

                if (existingTask != null)
                {
                    existingTask.Summary = externalIssue.Summary;
                    existingTask.OriginalEstimation = originalEstimation;
                    existingTask.SpentTime = spentTime;
                    existingTask.UserId = employeeId;
                    existingTask.EndDate = externalIssue.End;
                    existingTask.StartDate = externalIssue.Start;

                    _context.YandexTrackerTasks.Update(existingTask);
                }
                else
                {
                    var issue = new YandexTrackerTask()
                    {
                        Id = externalIssue.Id,
                        Summary = externalIssue.Summary,
                        OriginalEstimation = originalEstimation,
                        SpentTime = spentTime,
                        UserId = employeeId,
                        EndDate = externalIssue.End,
                        StartDate = externalIssue.Start
                    };

                    issuesDbModels.Add(issue);
                }
            }
            return issuesDbModels;
        }
    }
}
