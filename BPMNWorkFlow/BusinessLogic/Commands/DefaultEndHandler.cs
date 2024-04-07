using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultEndHandler : INodeHandler
    {
        void INodeHandler.Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId + " Executing End");
            processNode.ProcessInstance.SetOutputParameters(processNode);
            processNode.Done();
        }
    }
}