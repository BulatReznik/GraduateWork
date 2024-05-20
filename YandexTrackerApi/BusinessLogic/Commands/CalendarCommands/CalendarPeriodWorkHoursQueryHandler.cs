using BPMNWorkFlow.BusinessLogic.Models;
using MediatR;
using YandexTrackerApi.BusinessLogic.Models.CalendarModels;
using YandexTrackerApi.BusinessLogic.Models.CalendarQueries;
using YandexTrackerApi.DbModels;
using Microsoft.EntityFrameworkCore;

namespace YandexTrackerApi.BusinessLogic.Commands.CalendarCommands
{
    public class CalendarPeriodWorkHoursQueryHandler : IRequestHandler<CalendarPeriodWorkHoursQuery, Models.ResponseModel<CalendarPeriodWorkHoursResponse>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger<CalendarPeriodWorkHoursQueryHandler> _logger;

        public CalendarPeriodWorkHoursQueryHandler(IGraduateWorkContext context, ILogger<CalendarPeriodWorkHoursQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Models.ResponseModel<CalendarPeriodWorkHoursResponse>> Handle(CalendarPeriodWorkHoursQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var startDate = request.StartDate;
                var endDate = request.EndDate;

                // Получаем список дней за указанный период
                var calendarDays = await _context.CalendarData
                    .Where(d => d.Day >= startDate && d.Day <= endDate)
                    .ToListAsync(cancellationToken);

                // Общее количество рабочих часов
                var totalWorkHours = 0;

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var calendarDay = calendarDays.FirstOrDefault(d => d.Day == date);
                    if (calendarDay != null)
                    {
                        if (calendarDay.Holiday)
                        {
                            // Выходной день - 0 часов
                            continue;
                        }
                        if (calendarDay.Short)
                        {
                            // Короткий день - 7 часов
                            totalWorkHours += 7;
                        }
                    }
                    else
                    {
                        // Обычный рабочий день - 8 часов
                        totalWorkHours += 8;
                    }
                }

                var response = new CalendarPeriodWorkHoursResponse
                {
                    TotalWorkHours = totalWorkHours
                };

                return new Models.ResponseModel<CalendarPeriodWorkHoursResponse>
                {
                    Data = response
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось получить данные из календаря";
                _logger.LogError(ex, errorMessage);
                return new Models.ResponseModel<CalendarPeriodWorkHoursResponse>
                {
                    ErrorMessage = errorMessage
                };
            }
        }
    }
}
