using System.Collections.Immutable;
using System.Text.RegularExpressions;
using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeTypeCommands
{
    public class DefaultTaskHandler : INodeHandler
    {
        private readonly ITaskHandlerFactory _taskHandlerFactory;

        public DefaultTaskHandler(ITaskHandlerFactory taskHandlerFactory)
        {
            _taskHandlerFactory = taskHandlerFactory;
        }

        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            try
            {
                Console.WriteLine($"Выполнение задачи: Id: {processNode.NodeId} Имя задачи: {processNode.NodeName}");

                if (processNode.NodeName != null)
                {
                    var splitNodeName = processNode.NodeName.Split(':');
                    var taskName = splitNodeName[0];

                    if (splitNodeName.Length > 1)
                    {
                        var args = string.Join(":", splitNodeName.Skip(1));
                        var inputParameters = GetInputParameters(args);
                        processNode.InputParameters = processNode.InputParameters.AddRange(inputParameters);
                    }

                    var handler = await _taskHandlerFactory.GetTaskHandlerAsync(taskName);
                    await handler.ExecuteAsync(processNode, previousNode);
                }

                await processNode.DoneAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static IImmutableDictionary<string, object> GetInputParameters(string nodeName)
        {
            var parameters = ImmutableDictionary<string, object>.Empty;
            var regex = new Regex(@"\b(\w+):\s*([^\s]+)");
            var matches = regex.Matches(nodeName);

            foreach (Match match in matches)
            {
                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value;
                parameters = parameters.Add(key, value);
            }

            return parameters;
        }
    }

}
