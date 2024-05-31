using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    internal class DeleteRequestTaskHandler : ITaskHandler
    {
        public Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            throw new NotImplementedException();
        }
    }
}
