using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System.Text;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    public class LoadDataTaskHandler : ITaskHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            try
            {
                Console.WriteLine($"Выполнение задачи: Id: {processNode.NodeId} Имя задачи: {processNode.NodeName}");

                await LoadDataFromTaskTracker(processNode);

                // Устанавливаем флаг завершения задачи
                await processNode.DoneAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
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
