using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    internal class CompareTaskHandler : ITaskHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            // Получение значений для сравнения
            var obj1 = processNode.CurrentNodeInputParameters.FirstOrDefault(o => o.Key == "obj1").Value;
            var obj2 = processNode.CurrentNodeInputParameters.FirstOrDefault(o => o.Key == "obj2").Value;
            var comparisonSign = processNode.CurrentNodeInputParameters.FirstOrDefault(o => o.Key == "comparisonSign").Value;

            // Проверка наличия необходимых параметров
            if (obj1 == null || obj2 == null || comparisonSign == null)
            {
                Console.WriteLine("Недостаточно параметров для выполнения сравнения");
                return;
            }

            bool comparisonResult = false;

            // Выполнение сравнения в зависимости от знака
            switch (comparisonSign)
            {
                case "==":
                    comparisonResult = obj1 == obj2;
                    break;
                case "!=":
                    comparisonResult = obj1 != obj2;
                    break;
                case "<":
                    comparisonResult = Convert.ToDouble(obj1) < Convert.ToDouble(obj2);
                    break;
                case "<=":
                    comparisonResult = Convert.ToDouble(obj1) <= Convert.ToDouble(obj2);
                    break;
                case ">":
                    comparisonResult = Convert.ToDouble(obj1) > Convert.ToDouble(obj2);
                    break;
                case ">=":
                    comparisonResult = Convert.ToDouble(obj1) >= Convert.ToDouble(obj2);
                    break;
                default:
                    Console.WriteLine("Неподдерживаемый знак сравнения");
                    return;
            }

            // Выбор следующего узла в зависимости от результата сравнения
            string nextNodeKey = comparisonResult ? "TrueNode" : "FalseNode";

            var nextNode = processNode.NextNodes.FirstOrDefault(n => n.Key == nextNodeKey).Value;

            if (nextNode == null)
            {
                Console.WriteLine($"Следующий узел для ключа {nextNodeKey} не найден");
                return;
            }
        }
    }
}
