using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultSequenceHandler : INodeHandler
    {
        public void Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId + " Executing Sequence");
            bool result = true;

            if (processNode.Expression != null)
            {
                Console.WriteLine(processNode.NodeId + " Conditional Sequence");
                Console.WriteLine("Condition: " + processNode.Expression);
                var globals = new Globals(processNode.InputParameters.ToDictionary(e => e.Key, e => e.Value));
                try
                {
                    result = CSharpScript.EvaluateAsync<bool>(processNode.Expression, globals: globals).Result;
                    Console.WriteLine("Condition result: " + result.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (result)
            {
                processNode.Done();
            }
        }
    }
}