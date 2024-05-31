
namespace BPMNWorkFlow.BusinessLogic.Interfaces
{
    public interface ITaskHandlerFactory
    {
        Task<ITaskHandler> GetTaskHandlerAsync(string nodeName);
    }
}
