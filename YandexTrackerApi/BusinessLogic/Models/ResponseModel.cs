namespace YandexTrackerApi.BusinessLogic.Models
{
    public class ResponseModel<TValue>
    {
        public TValue? Data { get; init; }

        public string? ErrorMessage { get; set; }

        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    }
}
