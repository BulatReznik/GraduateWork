using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultExclusiveGatewayHandler : INodeHandler
    {
        void INodeHandler.Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId);
            processNode.Done();
        }
    }
}