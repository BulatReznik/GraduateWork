namespace YandexTrackerApi.Models.YandexModels
{
    public class Assignee
    {
        public string Self { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Display { get; set; } = null!;
        public string CloudUid { get; set; } = null!;
        public int PassportUid { get; set; }
    }
}
