using System.Collections.Concurrent;
using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeHandlers
{
    internal class DefaultInclusiveGatewayHandler : INodeHandler
    {
        ConcurrentDictionary<ProcessNode, ICollection<ProcessNode>> SequenceWait = new();

        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId);
            SequenceWait.GetOrAdd(processNode, new List<ProcessNode>(processNode.PreviousNodes));
            lock (SequenceWait[processNode])
            {
                SequenceWait[processNode].Remove(previousNode);
            }
            if (SequenceWait[processNode].Count == 0)
            {
                await processNode.DoneAsync();
            }
        }
    }
}