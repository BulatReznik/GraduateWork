using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultTaskHandler : INodeHandler
    {
        void INodeHandler.ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine($"Выполенение задачи: Id: {processNode.NodeId} Имя задачи: {processNode.NodeName}");

            if (processNode.NodeName == "Войти в таск-трекер")
            {
                Console.WriteLine("Произошел вход в таск-трекер");
            }

            if (processNode.NodeName == "Загрузить данные из таск-трекера")
            {
                Console.WriteLine("Данные из таск-трекера загружены");
            }

            if (processNode.NodeName == "Премировать сотрудника")
            {
                Console.WriteLine("Премировать сотрудника");
            }

            processNode.DoneAsync();
        }
    }
}