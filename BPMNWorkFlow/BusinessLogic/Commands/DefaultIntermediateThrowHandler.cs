﻿using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultIntermediateThrowHandler : INodeHandler
    {
        public async Task ExecuteAsync(ProcessNode currentNode, ProcessNode previousNode)
        {
            throw new NotImplementedException();
        }
    }
}
