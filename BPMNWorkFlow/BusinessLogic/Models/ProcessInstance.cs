using BPMNWorkFlow.BusinessLogic.Commands;
using BPMNWorkFlow.BusinessLogic.Interfaces;
using System.Collections.Immutable;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class ProcessInstance
    {
        public string Id { get; set; } = null!;

        public Process Process { get; } = null!;

        private IImmutableDictionary<string, object> inputParameters;

        public IImmutableDictionary<string, object> InputParameters
        {
            get
            {
                return inputParameters;
            }

            set
            {
                if (ValidParameters(value))
                    inputParameters = value;
                else
                    throw new Exception("Parameter type does not match process definition");
            }
        }

        public IImmutableDictionary<string, IImmutableDictionary<string, object>> OutputParameters { get; set; } = null!;
        public ProcessNode StartNode { get; internal set; } = null!;
        public IImmutableDictionary<string, ProcessNode> Nodes { get; set; } = null!;

        private IDictionary<string, INodeHandler> nodeHandlers = null!;
        public IDictionary<string, INodeHandler> NodeHandlers
        {
            get
            {
                return nodeHandlers;
            }

            set
            {
                if (ValidHandlers(value))
                    nodeHandlers = value;
                else
                    throw new Exception("Unhandled node type");
            }
        }

        public ProcessInstance(Process process)
        {
            Process = process;
        }

        public void Start()
        {
            StartNode.Execute(StartNode, null);
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
                nodeHandlers = new Dictionary<string, INodeHandler>();
                foreach (string n in Nodes.Values.Select(n => n.NodeType).Distinct())
                {
                    nodeHandlers.Add(n, defaultNodeHandlers[n]);
                }
            }
            else
                throw new Exception("Process contains an unknown node type");
        }

        public void SetHandler(string nodeType, INodeHandler nodeHandler)
        {
            nodeHandlers ??= new Dictionary<string, INodeHandler>();

            if (nodeHandlers.ContainsKey(nodeType))
                nodeHandlers[nodeType] = nodeHandler;
            else
                nodeHandlers.Add(nodeType, nodeHandler);
        }

        private bool ValidHandlers(IDictionary<string, INodeHandler> handlers)
        {
            var nodeTypes = Nodes.Values.Select(n => n.NodeType).Distinct();
            return nodeTypes.All(t => handlers.ContainsKey(t));
        }

        private bool ValidParameters(IImmutableDictionary<string, object> parameters)
        {
            var propertyMap = Process.Properties.ToDictionary(p => p.Name, p => p.StructureRef);
            return parameters.All(p => p.Value.GetType().Name.Equals(propertyMap[p.Key], StringComparison.CurrentCultureIgnoreCase));
        }

        public void Start(IDictionary<string, object> parameters)
        {
            //TODO Get node variables not process instance var
            InputParameters = parameters.ToImmutableDictionary();
            StartNode.InputParameters = parameters.ToImmutableDictionary();
            Start();
        }

        internal void SetOutputParameters(ProcessNode node)
        {
            OutputParameters ??= ImmutableDictionary.Create<string, IImmutableDictionary<string, object>>();

            OutputParameters.Add(node.NodeId, node.OutputParameters);
        }
    }
}
