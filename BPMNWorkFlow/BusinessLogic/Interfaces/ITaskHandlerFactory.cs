using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNWorkFlow.BusinessLogic.Interfaces
{
    public interface ITaskHandlerFactory
    {
        Task<ITaskHandler> GetTaskHandlerAsync(string nodeName);
    }
}
