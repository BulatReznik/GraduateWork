namespace BPMN.Models.Diagram
{
    public class DiagramRequest
    {
        public string ProjectId { get; set; } = null!;
        public string Name { get; set; }
        public string XMLDiagram { get; set; }
    }
}
