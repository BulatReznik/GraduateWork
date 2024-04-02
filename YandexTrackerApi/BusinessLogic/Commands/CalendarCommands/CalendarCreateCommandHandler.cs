using MediatR;
using Newtonsoft.Json;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.CalendarModels;
using YandexTrackerApi.BusinessLogic.Models.CalendarModels.CalendarDTO;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.CalendarCommands
{
    public class CalendarCreateCommandHandler : IRequestHandler<CalendarYearCreateCommand, ResponseModel<string>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;

        public CalendarCreateCommandHandler(ILogger logger, IGraduateWorkContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseModel<string>> Handle(CalendarYearCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                int currentYear = DateTime.Now.Year;

                if (!int.TryParse(request.Year, out var requestYear) || requestYear < 2013 || requestYear > currentYear)
                {
                    string errorMessage = $"Год должен быть в диапазоне от 2013 до {currentYear}";
                    _logger.LogInformation(errorMessage);
                    return new ResponseModel<string> { ErrorMessage = errorMessage };
                }

                var xmlCalenadarUrl = $"https://xmlcalendar.ru/data/ru/{requestYear}/calendar.json";

                using var httpClient = new HttpClient(); // добавленный блок using

                var response = await httpClient.GetAsync(xmlCalenadarUrl, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    var calendarData = JsonConvert.DeserializeObject<CalendarDataDto>(responseBody);

                    if (calendarData != null)
                    {
                        var calendarDbModels = ConvertToCalendarData(calendarData);
                        if (calendarDbModels != null)
                        {
                            await _context.CalendarData.AddRangeAsync(calendarDbModels, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);

                            return new ResponseModel<string> { Data = "Трудовой календарь был сохранен в базе данных" };
                        }
                        else
                        {
                            return new ResponseModel<string> { ErrorMessage = "Не удалось спарсить трудовой календарь" };
                        }
                    }
                    else
                    {
                        return new ResponseModel<string> { ErrorMessage = "Не удалось получить трудовой календарь с сервиса" };
                    }
                }
                else
                {
                    string errorMessage = $"Не удалось получить данные. Код состояния: {response.StatusCode}";
                    _logger.LogError(errorMessage);
                    return new ResponseModel<string> { ErrorMessage = errorMessage };
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось записать трудовой календарь";
                _logger.LogError(ex, errorMessage);
                return new ResponseModel<string> { ErrorMessage = errorMessage };
            }
        }

        /// <summary>
        /// Перевод календаря полученного сайта к календарю хранящемуся в бд
        /// </summary>
        /// <param name="calendarData"></param>
        /// <returns></returns>
        private List<CalendarDatum> ConvertToCalendarData(CalendarDataDto calendarData)
        {
            var calendarDataList = new List<CalendarDatum>();

            foreach (var month in calendarData.Months)
            {
                var monthDays = ParseDays(month.Days);
                foreach (var day in monthDays)
                {
                    bool isShort = day.Contains('*');
                    bool isHoliday = true;

                    if (isShort)
                        isHoliday = false;

                    calendarDataList.Add(new CalendarDatum
                    {
                        Day = new DateOnly(calendarData.Year, month.Month, int.Parse(day.TrimEnd('*', '+'))),
                        Holiday = isHoliday,
                        Short = isShort
                    });
                }
            }

            return calendarDataList;
        }

        /// <summary>
        /// Раздедение строки с днями
        /// </summary>
        /// <param name="daysString"></param>
        /// <returns></returns>
        private static List<string> ParseDays(string daysString)
        {
            return daysString.Split(',').ToList();
        }
    }
}
