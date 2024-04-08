using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultTimerEventHandler : INodeHandler
    {
        public void Execute(ProcessNode currentNode, ProcessNode previousNode)
        {
            throw new NotImplementedException();
        }
    }
}
