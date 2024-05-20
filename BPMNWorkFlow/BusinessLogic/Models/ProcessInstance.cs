using BPMNWorkFlow.BusinessLogic.Commands;
using BPMNWorkFlow.BusinessLogic.Interfaces;
using System.Collections.Immutable;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class ProcessInstance()
    {
        public ProcessInstance(Process process) : this()
        {
            Process = process;
            outputParametersBuilder = ImmutableDictionary<string, IImmutableDictionary<string, object>>.Empty.ToBuilder();
        }

        public string Id { get; set; } = null!;

        public Process Process { get; }

        private IImmutableDictionary<string, object> inputParameters;
        public IImmutableDictionary<string, object> InputParameters
        {
            get => inputParameters;

            private set
            {
                if (ValidParameters(value))
                    inputParameters = value;
                else
                    throw new Exception("Parameter type does not match process definition");
            }
        }

        private ImmutableDictionary<string, IImmutableDictionary<string, object>>.Builder outputParametersBuilder;
        public IImmutableDictionary<string, IImmutableDictionary<string, object>> OutputParameters => outputParametersBuilder.ToImmutable();

        public ProcessNode StartNode { get; internal set; } = null!;
        public IImmutableDictionary<string, ProcessNode> Nodes { get; set; } = null!;

        private IDictionary<string, INodeHandler> _nodeHandlers = null!;
        public IDictionary<string, INodeHandler> NodeHandlers
        {
            get => _nodeHandlers;

            set
            {
                if (ValidHandlers(value))
                    _nodeHandlers = value;
                else
                    throw new Exception("Unhandled node type");
            }
        }

        public void SetDefaultHandlers()
        {
            var defaultNodeHandlers = new Dictionary<string, INodeHandler>()
            {
                { "businessRuleTask", new DefaultBusinessRuleTaskHandler()},
                { "callActivity", new DefaultCallActivityHandler()},
                { "compensateEventDefinition", new DefaultCompensateEventHandler()},
                { "conditionalEventDefinition", new DefaultConditionalEventHandler()},
                { "complexGateway", new DefaultComplexGatewayHandler()},
                { "endEvent", new DefaultEndHandler()},
                { "errorEventDefinition", new DefaultErrorEventHandler()},
                { "escalationEventDefinition", new DefaultEscalationEventHandler()},
                { "eventBasedGateway", new DefaultEventBasedGatewayHandler()},
                { "exclusiveGateway", new DefaultExclusiveGatewayHandler()},
                { "inclusiveGateway", new DefaultInclusiveGatewayHandler()},
                { "intermediateCatchEvent", new DefaultIntermediateCatchHandler()},
                { "intermediateThrowEvent", new DefaultIntermediateThrowHandler()},
                { "laneSet", new DefaultLaneSetHandler()},
                { "linkEventDefinition", new DefaultLinkEventHandler()},
                { "manualTask", new DefaultManualTaskHandler()},
                { "messageEventDefinition", new DefaultMessageEventHandler()},
                { "parallelGateway", new DefaultParallelGatewayHandler()},
                { "receiveTask", new DefaultReceiveTaskHandler()},
                { "scriptTask", new DefaultScriptTaskHandler()},
                { "sendTask", new DefaultSendTaskHandler()},
                { "sequenceFlow", new DefaultSequenceHandler()},
                { "serviceTask", new DefaultServiceTaskHandler()},
                { "signalEventDefinition", new DefaultSignalEventHandler()},
                { "startEvent", new DefaultStartHandler()},
                { "subProcess", new DefaultSubProcessHandler()},
                { "task", new DefaultTaskHandler()},
                { "terminateEventDefinition", new DefaultTerminateEventHandler()},
                { "timerEventDefinition", new DefaultTimerEventHandler()},
                { "userTask", new DefaultUserTaskHandler()}
            };

            if (Nodes.All(t => defaultNodeHandlers.ContainsKey(t.Value.NodeType)))
            {
                _nodeHandlers = new Dictionary<string, INodeHandler>();
                foreach (var n in Nodes.Values.Select(n => n.NodeType).Distinct())
                {
                    _nodeHandlers.Add(n, defaultNodeHandlers[n]);
                }
            }
            else
                throw new Exception("Process contains an unknown node type");
        }

        public void SetHandler(string nodeType, INodeHandler nodeHandler)
        {
            _nodeHandlers ??= new Dictionary<string, INodeHandler>();

            if (_nodeHandlers.ContainsKey(nodeType))
                _nodeHandlers[nodeType] = nodeHandler;
            else
                _nodeHandlers.Add(nodeType, nodeHandler);
        }

        private bool ValidHandlers(IDictionary<string, INodeHandler> handlers)
        {
            var nodeTypes = Nodes.Values.Select(n => n.NodeType).Distinct();
            return nodeTypes.All(handlers.ContainsKey);
        }

        private bool ValidParameters(IImmutableDictionary<string, object> parameters)
        {
            var propertyMap = Process.Properties.ToDictionary(p => p.Name, p => p.StructureRef);
            return parameters.All(p => p.Value.GetType().Name.Equals(propertyMap[p.Key], StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task StartAsync(IDictionary<string, object> parameters)
        {
            //TODO Get node variables not process instance var
            InputParameters = parameters.ToImmutableDictionary();
            StartNode.InputParameters = parameters.ToImmutableDictionary();

            // Запускаем выполнение стартового узла
            await StartNode.Execute(StartNode, null);

            // Процесс завершен
            Console.WriteLine("Процесс завершен");
        }

        internal void SetOutputParameters(ProcessNode node)
        {
            outputParametersBuilder[node.NodeId] = node.OutputParameters;
        }
    }
}
