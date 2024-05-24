using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

                var isCheckForWorkedHours = previousNode.PreviousNodes
                    .Any(node => node.NodeName == "Определить количество часов списанные в задачи");

                var isCheckForEstimatedHours = previousNode.PreviousNodes
                    .Any(node => node.NodeName == "Определить кол-во часов, указанных для выполнения задачи");

                if (isCheckForWorkedHours || isCheckForEstimatedHours)
                {
                    await HandleTasksQuery(processNode, isCheckForWorkedHours, isCheckForEstimatedHours);
                }

                await processNode.DoneAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task HandleTasksQuery(ProcessNode processNode, bool isCheckForWorkedHours, bool isCheckForEstimatedHours)
        {
            if (!processNode.InputParameters.TryGetValue("StartDate", out var startDateObj) ||
                !processNode.InputParameters.TryGetValue("EndDate", out var endDateObj))
            {
                throw new ArgumentException("StartDate и EndDate обязательны в InputParameters");
            }

            var startDate = DateOnly.Parse(startDateObj.ToString() ?? string.Empty);
            var endDate = DateOnly.Parse(endDateObj.ToString() ?? string.Empty);

            var query = new
            {
                StartDate = startDate,
                EndDate = endDate
            };

            var content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7075/api/");
            var response = await httpClient.PostAsync("/api/v1/calendar/period/work/hours", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseModel = new ResponseModel<CalendarPeriodWorkHoursResponse>
                {
                    Data = JsonConvert.DeserializeObject<CalendarPeriodWorkHoursResponse>(responseContent)
                };

                if (responseModel.Data != null)
                {
                    var totalWorkHours = responseModel.Data.TotalWorkHours;
                    processNode.OutputParameters = processNode.OutputParameters
                        .Add("WorkHours", totalWorkHours);

                    var comparisonValue = isCheckForWorkedHours
                        ? processNode.InputParameters.TryGetValue("SpentTimeSum", out var spentTimeObj) ? int.Parse(spentTimeObj.ToString()) : 0
                        : processNode.InputParameters.TryGetValue("OriginalEstimationSum", out var estimationTimeObj) ? int.Parse(estimationTimeObj.ToString()) : 0;

                    string targetNodeName;
                    if (comparisonValue == totalWorkHours)
                    {
                        targetNodeName = "Часы равны количеству часов в трудовом календаре";
                    }
                    else if (comparisonValue < totalWorkHours)
                    {
                        targetNodeName = "Часы меньше количества часов в трудовом календаре";
                    }
                    else
                    {
                        targetNodeName = "Часы больше количества часов в трудовом календаре";
                    }

                    var nextNodesToRemove = new List<ProcessNode>();

                    foreach (var nextNode in processNode.NextNodes)
                    {
                        var nodesToRemove = nextNode.NextNodes
                            .Where(node => node.NodeName != targetNodeName)
                            .ToList();

                        foreach (var node in nodesToRemove)
                        {
                            nextNode.NextNodes.Remove(node);
                        }

                        if (nextNode.NextNodes.Count == 0)
                        {
                            nextNodesToRemove.Add(nextNode);
                        }
                    }

                    foreach (var nextNodeToRemove in nextNodesToRemove)
                    {
                        processNode.NextNodes.Remove(nextNodeToRemove);
                    }
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
    }

    public class CalendarPeriodWorkHoursResponse
    {
        public int TotalWorkHours { get; set; }
    }
}
