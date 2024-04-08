using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Collections.Immutable;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultScriptTaskHandler : INodeHandler
    {
        public void Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeId + " Executing Script");

            if (processNode.Expression != null)
            {
                Console.WriteLine("Script: " + processNode.Expression);
                var globals = new Globals(processNode.InputParameters.ToDictionary(e => e.Key, e => e.Value));
                try
                {
                    processNode.OutputParameters = CSharpScript
                        .EvaluateAsync<IDictionary<string, object>>(processNode.Expression, globals: globals)
                        .Result.ToImmutableDictionary();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            processNode.Done();
        }
    }
}