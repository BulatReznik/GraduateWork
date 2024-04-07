using System.Collections.Immutable;
using BPMNWorkFlow.BusinessLogic.Interfaces;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class ProcessNode
    {
        public string NodeId { get; set; } = null!;
        public string? NodeName { get; set; } = null!;
        public string NodeType { get; set; } = null!;
        public ProcessInstance ProcessInstance { get; set; } = null!;
        public IImmutableDictionary<string, object> InputParameters { get; set; } = null!;
        public IImmutableDictionary<string, object> OutputParameters { get; set; } = null!;
        public INodeHandler NodeHandler { get; set; } = null!;
        public ICollection<ProcessNode> NextNodes { get; set; } = null!;
        public ICollection<ProcessNode> PreviousNodes { get; set; } = null!;
        private Task Task { get; set; } = null!;
        public string Expression { get; set; } = null!;

        public ProcessNode(INodeHandler nodeHandler)
        {
            NodeHandler = nodeHandler;
        }

        public ProcessNode(string name, string type, string nodeName)
        {
            NodeId = name;
            NodeType = type;
            NodeName = nodeName;
        }

        public void Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            NodeHandler = ProcessInstance.NodeHandlers[NodeType];
            processNode.InputParameters ??= ProcessInstance.InputParameters;
            Task = new Task(() => NodeHandler.Execute(processNode, previousNode));
            Task.Start();
        }
        public void Done()
        {
            foreach (var node in NextNodes)
            {
                //to replace with variable resolution
                //for each node retrieve input parameters defined in BPMN
                //retrieve from node.OutputParameters (results of previous node)
                //retrieve missing necessary input from process variables
                node.InputParameters = OutputParameters;
                node.Execute(node, this);
            }
        }
    }
}