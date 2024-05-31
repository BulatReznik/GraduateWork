using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    internal class DeleteRequestTaskHandler : ITaskHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine("141241241421412424");

            processNode.OutputParameters = processNode.OutputParameters
                .Add("Tasks", "212411241224124124124214241");
        }
    }
}
