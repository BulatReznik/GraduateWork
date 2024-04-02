namespace YandexTrackerApi.BusinessLogic.Models.ProjectModels
{
    public class ProjectResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid CreatorId { get; set; }
    }
}
