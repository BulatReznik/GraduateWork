namespace BPMN.Models.Project
{
    public class ProjectResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid CreatorId { get; set; }
        public string CreatorName { get; set;} = null!;
    }
}
