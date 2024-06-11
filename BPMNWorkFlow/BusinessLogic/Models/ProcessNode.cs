using System.Collections.Immutable;
using BPMNWorkFlow.BusinessLogic.Interfaces;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class ProcessNode : EventArgs
    {
        /// <summary>
        /// Id узла
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// Текст внутри узла
        /// </summary>
        public string? NodeName { get; set; }

        /// <summary>
        /// Тип узда
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// К какому экземпляру относятся узлы
        /// </summary>
        public ProcessInstance ProcessInstance { get; set; } = null!;

        /// <summary>
        /// Входные параметры для узлов
        /// </summary>
        public IImmutableDictionary<string, object> InputParameters { get; set; }

        public IImmutableDictionary<string, object> CurrentNodeInputParameters { get; set; }

        /// <summary>
        /// Выходные параметры для узлов
        /// </summary>
        public IImmutableDictionary<string, object> OutputParameters { get; set; }

        /// <summary>
        /// Важные выходные параметры
        /// </summary>
        public IImmutableDictionary<string, object> ImportantParameters { get; set; }

        /// <summary>
        /// Обработчик этого узла
        /// </summary>
        private INodeHandler NodeHandler { get; set; } = null!;

        /// <summary>
        /// Следующие узлы
        /// </summary>
        public ICollection<ProcessNode> NextNodes { get; set; } = null!;

        /// <summary>
        /// Предыдущие узлы
        /// </summary>
        public ICollection<ProcessNode> PreviousNodes { get; set; } = null!;
        public string Expression { get; set; } = null!;
        public TaskCompletionSource<bool> TaskCompletionSource { get; } = new();

        public ProcessNode(string nodeId, string type, string nodeName)
        {
            NodeId = nodeId;
            NodeType = type;
            NodeName = nodeName;
            InputParameters = ImmutableDictionary<string, object>.Empty;
            OutputParameters = ImmutableDictionary<string, object>.Empty;
            ImportantParameters = ImmutableDictionary<string, object>.Empty;
            CurrentNodeInputParameters = ImmutableDictionary<string, object>.Empty;
        }

        public async Task Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            try
            {
                NodeHandler = ProcessInstance.NodeHandlers[NodeType];
                processNode.InputParameters ??= ProcessInstance.InputParameters;

                // Проверяем, не завершился ли уже TaskCompletionSource
                if (!TaskCompletionSource.Task.IsCompleted)
                {
                    await NodeHandler.ExecuteAsync(processNode, previousNode); // Дожидаемся выполнения узла

                    TaskCompletionSource
                        .SetResult(true); // Устанавливаем результат только если TaskCompletionSource не завершен
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task DoneAsync()
        {
            try
            {
                // Получаем следующий номер узла от ProcessInstance
                var nodeNumber = ProcessInstance.GetNextNodeNumber();

                // Добавляем текущий номер узла в OutputParameters
                OutputParameters = OutputParameters.Add("nodeNumber", nodeNumber);

                // Передаем текущие выходные параметры в ProcessInstance
                ProcessInstance.SetOutputParameters(this);
                ProcessInstance.SetImportantOutputParameter(this);

                foreach (var param in InputParameters)
                {
                    if (!OutputParameters.ContainsKey(param.Key))
                    {
                        OutputParameters = OutputParameters.Add(param.Key, param.Value);
                    }
                }

                foreach (var node in NextNodes)
                {
                    // Заменить на разрешение переменных
                    // Для каждого узла получить входные параметры, определенные в BPMN
                    // Получить их из node.OutputParameters (результаты предыдущего узла)
                    // Получить недостающие входные данные из переменных процесса
                    node.InputParameters = node.InputParameters.SetItems(OutputParameters);
                    await node.Execute(node, this);
                }
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e); 
            }
        }
    }
}