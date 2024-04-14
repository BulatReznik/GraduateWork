namespace YandexTrackerApi.BusinessLogic.Models.ProjectModels
{
    public class ProjectByIdResponse1
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public List<Diagram> Diagrams { get; set; }

        public sealed class Diagram()
        {
            public Guid Id { get; set; }
            public string Name { set; get; }
        }
    }
}
