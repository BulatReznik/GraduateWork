using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeTypeCommands
{
    internal class DefaultEndHandler : INodeHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId + " Executing End");
            await processNode.DoneAsync();
        }
    }
}