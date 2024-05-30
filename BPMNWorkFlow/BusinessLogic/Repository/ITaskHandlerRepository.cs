using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNWorkFlow.BusinessLogic.Repository
{
    public interface ITaskHandlerRepository
    {
        Task<string> GetHandlerClassNameAsync(string nodeName);
    }
}
