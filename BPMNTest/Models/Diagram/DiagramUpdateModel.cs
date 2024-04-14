namespace BPMN.Models.Diagram
{
    public class DiagramUpdateModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string XMLDiagram { get; set; }

        public Guid ProjectId { get; set; }
    }
}
