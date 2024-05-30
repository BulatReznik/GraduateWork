using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    internal class LoginTaskHandler : ITaskHandler
    {
        public Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            try
            {
                Console.WriteLine($"Выполнение задачи: Id: {processNode.NodeId} Имя задачи: {processNode.NodeName}");

                processNode.ImportantParameters.Add("Результат входа", "Произошел успешный вход");

                // Устанавливаем флаг завершения задачи
                await processNode.DoneAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
