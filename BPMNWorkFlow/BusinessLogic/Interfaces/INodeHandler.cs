using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Interfaces
{
    public interface INodeHandler
    {
        void ExecuteAsync(ProcessNode currentNode, ProcessNode previousNode);
    }
}