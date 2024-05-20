using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    internal class DefaultTaskHandler : INodeHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            try
            {
                Console.WriteLine($"Выполнение задачи: Id: {processNode.NodeId} Имя задачи: {processNode.NodeName}");

                // Извлекаем параметры из текста
                if (processNode.NodeName != null)
                {
                    // Разделяем NodeName на имя задачи и параметры
                    var splitNodeName = processNode.NodeName.Split(':');
                    var taskName = splitNodeName[0];

                    if (splitNodeName.Length > 1)
                    {
                        var args = string.Join(":", splitNodeName.Skip(1));
                        var inputParameters = GetInputParameters(args);
                        processNode.InputParameters = processNode.InputParameters.SetItems(inputParameters);
                    }

                    switch (taskName)
                    {
                        case "Войти в task-tracker":
                            // _taskTrackerService.LoginAsync();
                            Console.WriteLine("Произошел вход в task-tracker");
                            break;
                        case "Загрузить данные из task-tracker":
                            await LoadDataFromTaskTracker(processNode);
                            break;
                        case "Выбрать пользователя с именем":
                            if (processNode.InputParameters.TryGetValue("selectedUserName", out var userName))
                            {
                                Console.WriteLine($"Пользователь {userName} выбран");
                                processNode.OutputParameters =
                                    processNode.OutputParameters.Add("selectedUserName", userName);
                            }

                            break;
                        case "Установить начальную дату":
                            if (processNode.InputParameters.TryGetValue("StartDate", out var startDate))
                            {
                                var parsedStartDate = DateOnly.Parse(startDate.ToString());
                                Console.WriteLine($"Начальная дата установлена: {parsedStartDate}");
                                processNode.OutputParameters =
                                    processNode.OutputParameters.Add("StartDate", parsedStartDate);
                            }

                            break;
                        case "Установить конечную дату":
                            if (processNode.InputParameters.TryGetValue("EndDate", out var endDate))
                            {
                                var parsedEndDate = DateOnly.Parse(endDate.ToString());
                                Console.WriteLine($"Конечная дата установлена: {parsedEndDate}");
                                processNode.OutputParameters =
                                    processNode.OutputParameters.Add("EndDate", parsedEndDate);
                            }

                            break;
                        case "Определить количество часов списанные в задачи":
                            Console.WriteLine("Количество часов определено");
                            break;
                        default:
                            Console.WriteLine($"Неизвестная задача: {processNode.NodeName}");
                            break;
                    }
                }

                await processNode.DoneAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static IImmutableDictionary<string, object> GetInputParameters(string nodeName)
        {
            var parameters = ImmutableDictionary<string, object>.Empty;

            // Используем регулярное выражение для извлечения параметров из строки
            var regex = new Regex(@"\b(\w+):\s*([\w\-:]+)");
            var matches = regex.Matches(nodeName);

            foreach (Match match in matches)
            {
                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value;
                parameters = parameters.Add(key, value);
            }

            return parameters;
        }

        private async Task LoadDataFromTaskTracker(ProcessNode processNode)
        {
            try
            {
                /*
                if (!Guid.TryParse(processNode.InputParameters["UserId"]?.ToString(), out var userId))
                {
                    //throw new ArgumentException("Invalid UserId format");
                }

                if (!Guid.TryParse(processNode.InputParameters["ProjectId"]?.ToString(), out var projectId))
                {
                    //throw new ArgumentException("Invalid ProjectId format");
                }

                var employeeId = processNode.InputParameters["EmployeeId"]?.ToString();
                if (string.IsNullOrEmpty(employeeId))
                {
                    //throw new ArgumentException("EmployeeId is required");
                }*/

                if (!DateOnly.TryParse(processNode.InputParameters["StartDate"]?.ToString(), out var startDate))
                {
                    //throw new ArgumentException("Invalid StartDate format");
                }

                if (!DateOnly.TryParse(processNode.InputParameters["EndDate"]?.ToString(), out var endDate))
                {
                    //throw new ArgumentException("Invalid EndDate format");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
