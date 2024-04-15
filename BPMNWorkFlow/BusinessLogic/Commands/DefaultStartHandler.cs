using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultStartHandler : INodeHandler
    {
        public void ExecuteAsync(ProcessNode currentNode, ProcessNode previousNode)
        {
            Console.WriteLine($"Стартовое событие: Id узла: {currentNode.NodeId}");
            currentNode.DoneAsync().Wait();
        }
    }
}