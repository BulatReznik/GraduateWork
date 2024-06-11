using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    internal class ShowTextTaskHandler : ITaskHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            processNode.ImportantParameters = processNode.ImportantParameters.AddRange(processNode.CurrentNodeInputParameters);
        }
    }
}
