using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Interfaces
{
    public interface INodeHandler
    {
        void Execute(ProcessNode currentNode, ProcessNode previousNode);
    }
}