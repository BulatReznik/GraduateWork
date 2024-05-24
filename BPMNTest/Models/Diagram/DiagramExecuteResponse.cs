using System.Collections.Immutable;

namespace BPMN.Models.Diagram
{
    public class DiagramExecuteResponse
    {
        public string ExecutePath { get; set; }
        public IImmutableDictionary<string, IImmutableDictionary<string, object>> ImportantOutputParameters { get; set; }
    }
}
