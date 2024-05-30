using BPMNWorkFlow.BusinessLogic.Commands;
using BPMNWorkFlow.BusinessLogic.Interfaces;
using System.Collections.Immutable;
using BPMNWorkFlow.BusinessLogic.Commands.NodeHandlers;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class ProcessInstance()
    {
        private readonly ITaskHandlerFactory _taskHandlerFactory;

        public ProcessInstance(Process process, ITaskHandlerFactory taskHandlerFactory) : this()
        {
            Process = process;
            outputParametersBuilder =
                ImmutableDictionary<string, IImmutableDictionary<string, object>>.Empty.ToBuilder();
            importantOutputParametersBuilder =
                ImmutableDictionary<string, IImmutableDictionary<string, object>>.Empty.ToBuilder();
            _taskHandlerFactory = taskHandlerFactory;
        }

        /// <summary>
        /// Поле для счетчика номера узлов
        /// </summary>
        private int _nodeCounter = 0;

        public string Id { get; set; } = null!;

        private Process Process { get; } = null!;

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

        /// <summary>
        /// Выводимые параметры процесса
        /// </summary>
        public IImmutableDictionary<string, IImmutableDictionary<string, object>> OutputParameters => outputParametersBuilder.ToImmutable();

        private ImmutableDictionary<string, IImmutableDictionary<string, object>>.Builder importantOutputParametersBuilder;

        /// <summary>
        /// Важные выводимые параметры процесса
        /// </summary>
        public IImmutableDictionary<string, IImmutableDictionary<string, object>> ImportantOutputParameters => importantOutputParametersBuilder.ToImmutable();

        /// <summary>
        /// Первый узел
        /// </summary>
        public ProcessNode StartNode { get; internal set; } = null!;

        /// <summary>
        /// Все узлы
        /// </summary>
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
                { "task", new DefaultTaskHandler(taskHandlerFactory:_taskHandlerFactory)},
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
                throw new Exception("Процесс содержит узел неизвестного типа");
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

        /// <summary>
        /// Начало выполнения
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task StartAsync(IDictionary<string, object> parameters)
        {
            InputParameters = parameters.ToImmutableDictionary();
            StartNode.InputParameters = parameters.ToImmutableDictionary();

            // Запускаем выполнение стартового узла
            await StartNode.Execute(StartNode, null);

            // Процесс завершен
            Console.WriteLine("Процесс завершен");
        }

        /// <summary>
        /// Устанавливаем выходные параметры для всего процесса (ключ - имя узла, значении, выходные параметры)
        /// </summary>
        /// <param name="node"></param>
        internal void SetOutputParameters(ProcessNode node)
        {
            outputParametersBuilder[node.NodeId] = node.OutputParameters;
        }

        /// <summary>
        /// Устанавливаем важные выходные параметры для всего процесса (ключ - имя узла, значении, выходные параметры)
        /// </summary>
        /// <param name="node"></param>
        internal void SetImportantOutputParameter(ProcessNode node)
        {
            if (node.ImportantParameters.Count > 0)
            {
                importantOutputParametersBuilder[node.NodeId] = node.ImportantParameters;
            }
        }

        /// <summary>
        /// Метод для получения нового номера узла
        /// </summary>
        /// <returns></returns>
        public int GetNextNodeNumber()
        {
            return ++_nodeCounter;
        }
    }
}
