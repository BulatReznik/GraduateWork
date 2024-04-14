namespace BPMN.Models.Diagram
{
    public class DiagramResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Document { get; set; } = null!;
    }
}
