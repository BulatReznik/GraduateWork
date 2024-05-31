using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    public class PostRequestTaskHandler : ITaskHandler
    {
        public Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            throw new NotImplementedException();
        }
    }
}
