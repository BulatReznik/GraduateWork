using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultStartHandler : INodeHandler
    {
        void INodeHandler.Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId + " Executing Start");
            processNode.Done();
        }
    }
}