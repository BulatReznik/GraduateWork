using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System.Text.Json;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    public class DeleteRequestTaskHandler : ITaskHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            // Получение URL API из параметров узла
            var apiUrl = processNode.InputParameters
                .FirstOrDefault(ip => ip.Key == "HREF").Value?.ToString();

            if (string.IsNullOrEmpty(apiUrl))
            {
                Console.WriteLine("API URL не найден в InputParameters");
                return;
            }

            // Создание HttpClient
            using var httpClient = new HttpClient();

            // Создание запроса
            var request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);

            try
            {
                // Выполнение запроса
                var response = await httpClient.SendAsync(request);

                // Чтение и десериализация ответа
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseBody))
                {
                    var responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

                    if (responseData != null)
                    {
                        foreach (var kvp in responseData)
                        {
                            processNode.OutputParameters = processNode.OutputParameters.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Ошибка запроса: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Общая ошибка: {e.Message}");
            }
        }
    }
}
