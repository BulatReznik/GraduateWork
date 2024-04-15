using System.Collections.Immutable;
using BPMNWorkFlow.BusinessLogic.Interfaces;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class ProcessNode : EventArgs
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
        public string Expression { get; set; } = null!;
        public TaskCompletionSource<bool> TaskCompletionSource { get; set; } = new TaskCompletionSource<bool>();

        public ProcessNode(string name, string type, string nodeName)
        {
            NodeId = name;
            NodeType = type;
            NodeName = nodeName;
        }

        public async Task Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            NodeHandler = ProcessInstance.NodeHandlers[NodeType];
            processNode.InputParameters ??= ProcessInstance.InputParameters;

            // Проверяем, не завершился ли уже TaskCompletionSource
            if (!TaskCompletionSource.Task.IsCompleted)
            {
                NodeHandler.ExecuteAsync(processNode, previousNode); // Дожидаемся выполнения узла

                TaskCompletionSource.SetResult(true); // Устанавливаем результат только если TaskCompletionSource не завершен
            }
        }

        public async Task DoneAsync()
        {
            foreach (var node in NextNodes)
            {
                //to replace with variable resolution
                //for each node retrieve input parameters defined in BPMN
                //retrieve from node.OutputParameters (results of previous node)
                //retrieve missing necessary input from process variables
                node.InputParameters = OutputParameters;
                await node.Execute(node, this);
            }
        }

        public class NodeCompletedEventArgs : EventArgs
        {
            public string NodeId { get; }
            public string NodeName { get; }

            public NodeCompletedEventArgs(string nodeId, string nodeName)
            {
                NodeId = nodeId;
                NodeName = nodeName;
            }
        }
    }
}