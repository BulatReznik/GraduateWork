﻿using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultManualTaskHandler : INodeHandler
    {
        public void Execute(ProcessNode currentNode, ProcessNode previousNode)
        {
            throw new NotImplementedException();
        }
    }
}
