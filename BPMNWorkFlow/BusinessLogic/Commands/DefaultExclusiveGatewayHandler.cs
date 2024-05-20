using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultExclusiveGatewayHandler : INodeHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            try
            {
                Console.WriteLine(processNode.NodeId);

                Console.WriteLine(processNode.NodeName);

                if (previousNode.PreviousNodes.Any(previousNode => previousNode.NodeName == "Определить количество часов списанные в задачи"))
                {
                    var name = processNode.NodeName;

                    var nextNodesToRemove = new List<ProcessNode>();

                    foreach (var nextNode in processNode.NextNodes)
                    {
                        // Находим узлы, которые нужно удалить у текущего следующего узла
                        var nodesToRemove = nextNode.NextNodes
                            .Where(node => node.NodeName != "Часы больше количества часов в трудовом календаре")
                            .ToList();

                        // Удаляем найденные узлы из следующего узла
                        foreach (var node in nodesToRemove)
                        {
                            nextNode.NextNodes.Remove(node);
                        }

                        // Если после удаления узлов нет следующих узлов, добавляем текущий узел для удаления
                        if (nextNode.NextNodes.Count == 0)
                        {
                            nextNodesToRemove.Add(nextNode);
                        }
                    }

                    // Удаляем узлы из processNode.NextNodes, если у них нет следующих узлов
                    foreach (var nextNodeToRemove in nextNodesToRemove)
                    {
                        processNode.NextNodes.Remove(nextNodeToRemove);
                    }
                }


                if (previousNode.PreviousNodes.Any(previousNode => previousNode.NodeName == "Определить кол-во часов, указанных для выполнения задачи"))
                {
                    var nodesToRemove = new List<ProcessNode>();

                    foreach (var nextNode in processNode.NextNodes)
                    {
                        // Находим узлы, которые нужно удалить у текущего следующего узла
                        var nodesToRemoveFromNextNode = nextNode.NextNodes
                            .Where(node => node.NodeName != "Часы меньше количества часов в трудовом календаре")
                            .ToList();

                        // Удаляем найденные узлы из следующего узла
                        foreach (var nodeToRemove in nodesToRemoveFromNextNode)
                        {
                            nextNode.NextNodes.Remove(nodeToRemove);
                        }

                        // Если после удаления узлов нет следующих узлов, добавляем текущий узел для удаления
                        if (nextNode.NextNodes.Count == 0)
                        {
                            nodesToRemove.Add(nextNode);
                        }
                    }

                    // Удаляем узлы из processNode.NextNodes, если у них нет следующих узлов
                    foreach (var nodeToRemove in nodesToRemove)
                    {
                        processNode.NextNodes.Remove(nodeToRemove);
                    }
                }

                await processNode.DoneAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}