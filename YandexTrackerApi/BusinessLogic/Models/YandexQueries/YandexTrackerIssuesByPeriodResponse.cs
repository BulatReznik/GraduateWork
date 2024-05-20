namespace YandexTrackerApi.BusinessLogic.Models.YandexQueries
{
    public class YandexTrackerIssuesByPeriodResponse
    {
        public List<YandexTask> Tasks { get; set; }
        public int OriginalEstimationSum { get; set; }
        public int SpentTimeSum { get; set; }
    }

    public class YandexTask
    {
        public string Id { get; set; }
        public string? Summary { get; set; }
        public int OriginalEstimation { get; set; }
        public int SpentTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
