using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    internal class Property
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StructureRef { get; set; }
        public bool IsCollection { get; set; }

        public Property(string id, string name, string structureRef, bool isCollection)
        {
            Id = id;
            Name = name;
            StructureRef = structureRef;
            IsCollection = isCollection;
        }
    }
}
