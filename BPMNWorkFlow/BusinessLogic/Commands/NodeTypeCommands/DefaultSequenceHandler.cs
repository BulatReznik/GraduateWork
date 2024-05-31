using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeTypeCommands
{
    internal class DefaultSequenceHandler : INodeHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            try
            {
                Console.WriteLine(processNode.NodeId + " Executing Sequence");
                var result = true;

                if (processNode.Expression != null)
                {
                    Console.WriteLine(processNode.NodeId + " Conditional Sequence");
                    Console.WriteLine("Condition: " + processNode.Expression);
                    var globals = new Globals(processNode.InputParameters.ToDictionary(e => e.Key, e => e.Value));
                    try
                    {
                        result = CSharpScript.EvaluateAsync<bool>(processNode.Expression, globals: globals).Result;
                        Console.WriteLine("Condition result: " + result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                if (result)
                {
                    await processNode.DoneAsync();
                }
            }
            catch (Exception a)
            {
                Console.WriteLine(a);
                throw;
            }
        }
    }
}