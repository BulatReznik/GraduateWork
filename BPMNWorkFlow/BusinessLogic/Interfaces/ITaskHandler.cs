using BPMNWorkFlow.BusinessLogic.Models;
namespace BPMNWorkFlow.BusinessLogic.Interfaces
{
    public interface ITaskHandler
    {
        Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode);
    }
}
