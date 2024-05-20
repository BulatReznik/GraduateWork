using MediatR;
using YandexTrackerApi.BusinessLogic.Models.CalendarModels;

namespace YandexTrackerApi.BusinessLogic.Models.CalendarQueries
{
    public class CalendarPeriodWorkHoursQuery : IRequest<ResponseModel<CalendarPeriodWorkHoursResponse>>
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
