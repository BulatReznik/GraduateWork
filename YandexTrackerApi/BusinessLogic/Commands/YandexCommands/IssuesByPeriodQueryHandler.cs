using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.YandexQueries;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.YandexCommands
{
    public class IssuesByPeriodQueryHandler : IRequestHandler<YandexTrackerIssuesByPeriodQuery, ResponseModel<YandexTrackerIssuesByPeriodResponse>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger<IssuesByPeriodQueryHandler> _logger;

        public IssuesByPeriodQueryHandler(IGraduateWorkContext context, ILogger<IssuesByPeriodQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseModel<YandexTrackerIssuesByPeriodResponse>> Handle(YandexTrackerIssuesByPeriodQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем доступ пользователя к проекту
                var access = await _context.UsersProjects
                    .AnyAsync(up => up.UserId == request.UserId
                                    && up.ProjectId == request.ProjectId, cancellationToken);

                if (!access)
                {
                    return new ResponseModel<YandexTrackerIssuesByPeriodResponse> { ErrorMessage = "У пользователя нет доступа к этому проекту" };
                }

                // Поиск ID пользователя по логину
                var userId = await _context.YandexTrackerUsers
                    .Where(u => u.Login == request.UserName)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (string.IsNullOrEmpty(userId))
                {
                    return new ResponseModel<YandexTrackerIssuesByPeriodResponse> { ErrorMessage = "Пользователь не найден" };
                }

                // Поиск задач по Id пользователя и указанному периоду
                var tasks = await _context.YandexTrackerTasks
                    .Where(t => t.UserId == userId
                                && t.StartDate >= request.StartDate
                                && t.EndDate <= request.EndDate)
                    .ToListAsync(cancellationToken);

                if (!tasks.Any())
                {
                    return new ResponseModel<YandexTrackerIssuesByPeriodResponse> { ErrorMessage = "Задачи не найдены" };
                }

                // Рассчитываем сумму часов оценки и потраченного времени
                var totalEstimationSum = tasks.Sum(t => t.OriginalEstimation ?? 0);
                var spentTimeSum = tasks.Sum(t => t.SpentTime ?? 0);

                var response = new YandexTrackerIssuesByPeriodResponse
                {
                    Tasks = tasks.Select(t => new YandexTask
                    {
                        Id = t.Id,
                        Summary = t.Summary,
                        OriginalEstimation = t.OriginalEstimation ?? 0,
                        SpentTime = t.SpentTime ?? 0,
                        StartDate = t.StartDate,
                        EndDate = t.EndDate
                    }).ToList(),
                    OriginalEstimationSum = totalEstimationSum,
                    SpentTimeSum = spentTimeSum
                };

                return new ResponseModel<YandexTrackerIssuesByPeriodResponse>
                {
                    Data = response
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось получить задачи из базы данных";
                _logger.LogError(ex, errorMessage);
                return new ResponseModel<YandexTrackerIssuesByPeriodResponse> { ErrorMessage = errorMessage };
            }
        }
    }
}
