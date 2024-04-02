using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.CalendarModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.CalendarCommands
{
    public class CalendarDayCreateCommandHandler : IRequestHandler<CalendarDayCreateCommand, ResponseModel<string>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger _logger;

        public CalendarDayCreateCommandHandler(IGraduateWorkContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseModel<string>> Handle(CalendarDayCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем доступ пользователя к проекту
                if (!await UserHasAccessToProject(request.UserId, request.ProjectId, cancellationToken))
                {
                    var errorMessage = "У пользователя нет доступа к этому проекту";
                    _logger.LogInformation(errorMessage);
                    return new ResponseModel<string> { ErrorMessage = errorMessage };
                }

                // Проверяем существование работника в проекте
                if (!await EmployeeExistsInProject(request.EmployeeId, request.ProjectId, cancellationToken))
                {
                    return new ResponseModel<string> { ErrorMessage = "Работник не был найден" };
                }

                // Добавляем выходные дни в соответствии с указанным периодом
                if (request.DateStart == request.DateFinish)
                {
                    // Если дата начала и окончания равны, добавляем только один день
                    await AddSingleHoliday(request.DateStart, request.EmployeeId, cancellationToken);
                }
                else
                {
                    // Если дата начала и окончания различны, добавляем все дни в периоде
                    await AddMultipleHolidays(request.DateStart, request.DateFinish, request.EmployeeId, cancellationToken);
                }

                return new ResponseModel<string> { Data = "Данные о внесении выходных дней были записаны" };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось добавть данные о выходном";
                _logger.LogError(errorMessage, ex);
                return new ResponseModel<string>() { ErrorMessage = errorMessage };
            }
        }

        /// <summary>
        /// Метод для проверки доступа пользователя к проекту
        /// </summary>
        private async Task<bool> UserHasAccessToProject(Guid userId, Guid projectId, CancellationToken cancellationToken)
        {
            return await _context.UsersProjects
                .AnyAsync(up => up.UserId == userId && up.ProjectId == projectId, cancellationToken);
        }

        /// <summary>
        /// Метод для проверки существования работника в проекте
        /// </summary>
        private async Task<bool> EmployeeExistsInProject(Guid employeeId, Guid projectId, CancellationToken cancellationToken)
        {
            return await _context.YandexTrackerUsers
                .AnyAsync(ytu => ytu.Id == employeeId && ytu.ProjectId == projectId, cancellationToken);
        }

        /// <summary>
        /// Метод для добавления выходного дня, если указан только один день
        /// </summary>
        private async Task AddSingleHoliday(DateOnly holidayDate, Guid employeeId, CancellationToken cancellationToken)
        {
            var yandexTrackerUserHolidaysDbModel = new YandexTrackerUserHoliday()
            {
                Day = holidayDate,
                UserId = employeeId
            };
            await _context.YandexTrackerUserHolidays.AddAsync(yandexTrackerUserHolidaysDbModel, cancellationToken);
        }

        /// <summary>
        /// Метод для добавления выходных дней в указанном периоде
        /// </summary>
        private async Task AddMultipleHolidays(DateOnly startDate, DateOnly endDate, Guid employeeId, CancellationToken cancellationToken)
        {
            var daysToAdd = new List<YandexTrackerUserHoliday>();
            var currentDate = startDate;
            while (currentDate <= endDate)
            {
                var yandexTrackerUserHolidayDbModel = new YandexTrackerUserHoliday()
                {
                    Day = currentDate,
                    UserId = employeeId
                };
                daysToAdd.Add(yandexTrackerUserHolidayDbModel);
                currentDate = currentDate.AddDays(1);
            }
            await _context.YandexTrackerUserHolidays.AddRangeAsync(daysToAdd, cancellationToken);
        }
    }
}
