using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultEndHandler : INodeHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId + " Executing End");
            processNode.OutputParameters = processNode.InputParameters;
            processNode.ProcessInstance.SetOutputParameters(processNode);
            await processNode.DoneAsync();
        }
    }
}