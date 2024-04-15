using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultEndHandler : INodeHandler
    {
        public void ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId + " Executing End");
            processNode.ProcessInstance.SetOutputParameters(processNode);
            processNode.DoneAsync();
        }
    }
}