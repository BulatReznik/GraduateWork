namespace BPMN.Models.Project
{
    public class ProjectUserResponse
    {
        public Guid UserId { get; set; }
        public bool? Confirmed { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
    }
}
