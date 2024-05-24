using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
                        processNode.InputParameters = processNode.InputParameters.AddRange(inputParameters);
                    }

                    var key = previousNode?.PreviousNodes?.FirstOrDefault()?.NodeName;
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
                            if (processNode.InputParameters.TryGetValue("UserName", out var userName))
                            {
                                Console.WriteLine($"Пользователь {userName} выбран");
                                processNode.OutputParameters = processNode.OutputParameters.Add("UserName", userName);
                            }

                            break;
                        case "Установить начальную дату":
                            if (processNode.InputParameters.TryGetValue("StartDate", out var startDate))
                            {
                                var parsedStartDate = DateOnly.Parse(startDate.ToString());
                                Console.WriteLine($"Начальная дата установлена: {parsedStartDate}");
                                processNode.OutputParameters = processNode.OutputParameters.Add("StartDate", parsedStartDate);
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

                        case "Часы равны количеству часов в трудовом календаре":
                            if (key != null)
                            {
                                processNode.ImportantParameters =
                                    processNode.ImportantParameters.Add($"{key}",
                                        "Часы равны количеству часов в трудовом календаре");
                            }
                            break;
                        case "Часы меньше количества часов в трудовом календаре":
                            if (key != null)
                            {
                                processNode.ImportantParameters =
                                    processNode.ImportantParameters.Add($"{key}",
                                         "Часы меньше количества часов в трудовом календаре");
                            }
                            break;
                        case "Часы больше количества часов в трудовом календаре":
                            if (key != null)
                            {
                                processNode.ImportantParameters =
                                    processNode.ImportantParameters.Add($"{key}",
                                         "Часы больше количества часов в трудовом календаре");
                            }
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
            var regex = new Regex(@"\b(\w+):\s*([^\s]+)");
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
                if (!DateOnly.TryParse(processNode.InputParameters["StartDate"]?.ToString(), out var startDate))
                {
                    throw new ArgumentException("Invalid StartDate format");
                }

                if (!DateOnly.TryParse(processNode.InputParameters["EndDate"]?.ToString(), out var endDate))
                {
                    throw new ArgumentException("Invalid EndDate format");
                }

                var query = new
                {
                    UserName = processNode.InputParameters["UserName"]?.ToString(),
                    StartDate = startDate,
                    EndDate = endDate,
                    ProjectId = Guid.Parse("ca899182-b68d-4284-a0d8-7268f428a6cc") //Guid.Parse(processNode.InputParameters["ProjectId"]?.ToString())
                };

                var content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");

                var _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri("https://localhost:7075/api/");
                var response = await _httpClient.PostAsync("/api/v1/yandex/user/tasks", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var responseModel = new ResponseModel<YandexTrackerIssuesByPeriodResponse>
                    {
                        Data = JsonConvert.DeserializeObject<YandexTrackerIssuesByPeriodResponse>(responseContent)
                    };

                    if (responseModel.Data != null)
                    {
                        processNode.OutputParameters = processNode.OutputParameters
                            .Add("Tasks", responseModel.Data.Tasks)
                            .Add("OriginalEstimationSum", responseModel.Data.OriginalEstimationSum)
                            .Add("SpentTimeSum", responseModel.Data.SpentTimeSum);
                    }
                    else
                    {
                        throw new Exception("Не удалось получить данные задач");
                    }
                }
                else
                {
                    throw new Exception("Не удалось получить ответ от сервера");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка при загрузке данных из task-tracker");
                throw;
            }
        }
    }
}
