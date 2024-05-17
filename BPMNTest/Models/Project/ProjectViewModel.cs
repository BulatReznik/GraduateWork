namespace BPMN.Models.Project
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatorId { get; set; }
        public List<DiagramViewModel> Diagrams { get; set; }
        public string CreatorName { get; set; }
    }

    public class DiagramViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
    }
}
