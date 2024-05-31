using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeTypeCommands
{
    internal class DefaultBusinessRuleTaskHandler : INodeHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId + " Executing BusinessRule");
            await processNode.DoneAsync();
        }
    }
}