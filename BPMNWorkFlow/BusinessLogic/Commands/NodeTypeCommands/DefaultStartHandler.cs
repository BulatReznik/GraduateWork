using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeTypeCommands
{
    internal class DefaultStartHandler : INodeHandler
    {
        public async Task ExecuteAsync(ProcessNode currentNode, ProcessNode previousNode)
        {
            Console.WriteLine($"Стартовое событие: Id узла: {currentNode.NodeId}");
            currentNode.DoneAsync().Wait();
        }
    }
}