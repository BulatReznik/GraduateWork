using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Interfaces
{
    public interface INodeHandler
    {
        Task ExecuteAsync(ProcessNode currentNode, ProcessNode previousNode);
    }
}