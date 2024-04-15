using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultExclusiveGatewayHandler : INodeHandler
    {
        public void ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId);

            Console.WriteLine(processNode.NodeName);

            foreach (var node in processNode.NextNodes)
            {
                if (node.NodeName == "Да")
                {
                    Console.WriteLine("Результат свитча равен да");
                }
            }

            processNode.DoneAsync();
        }
    }
}