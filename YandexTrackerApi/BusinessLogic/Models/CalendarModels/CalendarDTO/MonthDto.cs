namespace YandexTrackerApi.BusinessLogic.Models.CalendarModels.CalendarDTO
{
    public class MonthDto
    {
        public int Month { get; set; }
        public string Days { get; set; } // Изменили тип свойства на List<string>
    }
}
