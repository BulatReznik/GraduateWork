using BPMNWorkFlow.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNWorkFlow.BusinessLogic.Interfaces
{
    public interface ITaskHandler
    {
        Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode);
    }
}
