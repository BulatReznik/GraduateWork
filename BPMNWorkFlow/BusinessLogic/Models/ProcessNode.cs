using System.Collections.Immutable;
using BPMNWorkFlow.BusinessLogic.Interfaces;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class ProcessNode : EventArgs
    {
        public string NodeId { get; set; }
        public string? NodeName { get; set; }
        public string NodeType { get; set; }
        public ProcessInstance ProcessInstance { get; set; } = null!;
        public IImmutableDictionary<string, object> InputParameters { get; set; }
        public IImmutableDictionary<string, object> OutputParameters { get; set; }
        private INodeHandler NodeHandler { get; set; } = null!;
        public ICollection<ProcessNode> NextNodes { get; set; } = null!;
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
    }
}