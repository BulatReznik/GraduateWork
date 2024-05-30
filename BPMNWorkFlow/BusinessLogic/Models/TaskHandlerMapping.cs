using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class TaskHandlerMapping
    {
        public int Id { get; set; }
        public string NodeName { get; set; }
        public string HandlerClassName { get; set; }
    }
}
