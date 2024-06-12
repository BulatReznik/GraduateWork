using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    internal class NoOpTaskHandler : ITaskHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            processNode.ImportantParameters = processNode.ImportantParameters.Add("Text", "Не было найдено обработчика!");
        }
    }
}
