using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeTypeCommands
{
    internal class DefaultExclusiveGatewayHandler : INodeHandler
    {
        private readonly ITaskHandlerFactory _taskHandlerFactory;

        public DefaultExclusiveGatewayHandler(ITaskHandlerFactory taskHandlerFactory)
        {
            _taskHandlerFactory = taskHandlerFactory;
        }

        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            try
            {
                Console.WriteLine($"Выполнение развилки: Id: {processNode.NodeId} Имя развилки: {processNode.NodeName}");

                if (processNode.NodeName != null)
                {
                    var splitNodeName = processNode.NodeName.Split(':');
                    var taskName = splitNodeName[0];

                    if (splitNodeName.Length > 1)
                    {
                        var args = string.Join(":", splitNodeName.Skip(1));
                        processNode.CurrentNodeInputParameters = processNode.CurrentNodeInputParameters.AddRange(GetInputParameters(args));
                        processNode.InputParameters = UpdateInputParameters(processNode.InputParameters, processNode.CurrentNodeInputParameters);
                    }

                    var handler = await _taskHandlerFactory.GetTaskHandlerAsync(taskName);

                    await handler.ExecuteAsync(processNode, previousNode);
                }

                await processNode.DoneAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private static IImmutableDictionary<string, object> GetInputParameters(string nodeName)
        {
            var parameters = ImmutableDictionary<string, object>.Empty;
            var regex = new Regex(@"(\w+):\s*""([^""]+)""");
            var matches = regex.Matches(nodeName);

            foreach (Match match in matches)
            {
                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value;
                parameters = parameters.Add(key, value);
            }

            return parameters;
        }

        private static IImmutableDictionary<string, object> UpdateInputParameters(
            IImmutableDictionary<string, object> existingParameters,
            IImmutableDictionary<string, object> newParameters)
        {
            var updatedParameters = existingParameters;
            foreach (var kvp in newParameters)
            {
                updatedParameters = updatedParameters.SetItem(kvp.Key, kvp.Value);
            }
            return updatedParameters;
        }
    }
}
