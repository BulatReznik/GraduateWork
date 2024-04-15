using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class NodeExecutedEventArgs : EventArgs
    {
        public ProcessNode ExecutedNode { get; }

        public NodeExecutedEventArgs(ProcessNode executedNode)
        {
            ExecutedNode = executedNode;
        }
    }
}
