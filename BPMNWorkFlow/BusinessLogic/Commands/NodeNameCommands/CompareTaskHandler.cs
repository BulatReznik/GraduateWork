using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    internal class CompareTaskHandler : ITaskHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            // Получение значений для сравнения
            var obj1Key = processNode.CurrentNodeInputParameters.FirstOrDefault(o => o.Key == "obj1").Value?.ToString();
            var obj2Key = processNode.CurrentNodeInputParameters.FirstOrDefault(o => o.Key == "obj2").Value?.ToString();

            if (string.IsNullOrEmpty(obj1Key) || string.IsNullOrEmpty(obj2Key))
            {
                Console.WriteLine("Ключи obj1 или obj2 не найдены или пусты");
                return;
            }

            var obj1 = previousNode.InputParameters.FirstOrDefault(ip => ip.Key == obj1Key).Value;
            var obj2 = previousNode.InputParameters.FirstOrDefault(ip => ip.Key == obj2Key).Value;

            // Проверка наличия необходимых параметров
            if (obj1 == null || obj2 == null)
            {
                Console.WriteLine("Недостаточно параметров для выполнения сравнения");
                return;
            }

            // Преобразование значений в числа для сравнения
            if (!double.TryParse(obj1.ToString(), out var value1) || !double.TryParse(obj2.ToString(), out var value2))
            {
                Console.WriteLine("Невозможно преобразовать параметры к числовым значениям");
                return;
            }

            // Определение знака сравнения
            string nextNodeName;

            if (value1 < value2)
            {
                nextNodeName = "<";
            }
            else if (value1 > value2)
            {
                nextNodeName = ">";
            }
            else
            {
                nextNodeName = "==";
            }

            // Поиск следующего узла на основе имени
            var nextNode = processNode.NextNodes.FirstOrDefault(n => n.NodeName == nextNodeName);

            if (nextNode == null)
            {
                Console.WriteLine($"Следующий узел для знака {nextNodeName} не найден");
                return;
            }

            // Удаление всех узлов, которые не соответствуют знаку сравнения
            processNode.NextNodes = processNode.NextNodes
                .Where(n => n.NodeName == nextNodeName).ToList();

            await processNode.DoneAsync();
        }
    }
}
