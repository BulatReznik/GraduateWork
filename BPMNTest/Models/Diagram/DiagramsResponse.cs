namespace BPMN.Models.Diagram
{
    public class DiagramsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? Date { get; set; }
    }
}
