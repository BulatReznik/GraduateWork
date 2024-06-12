using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System.Text.Json;
using System.Text;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    public class PutRequestTaskHandler : ITaskHandler
    {
        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            // Получение URL API и параметров запроса
            var apiUrl = processNode.InputParameters
                .FirstOrDefault(ip => ip.Key == "HREF").Value?.ToString();

            if (string.IsNullOrEmpty(apiUrl))
            {
                Console.WriteLine("API URL не найден в InputParameters");
                return;
            }

            // Преобразование InputParameters в словарь для использования в запросе
            var requestParameters = processNode.InputParameters
                .Where(ip => ip.Key != "HREF")
                .ToDictionary(ip => ip.Key, ip => ip.Value?.ToString());

            // Создание JSON-контента для запроса
            var jsonContent = JsonSerializer.Serialize(requestParameters);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Создание HttpClient
            using var httpClient = new HttpClient();

            // Создание запроса
            var request = new HttpRequestMessage(HttpMethod.Put, apiUrl)
            {
                Content = content
            };

            try
            {
                // Выполнение запроса
                var response = await httpClient.SendAsync(request);

                // Чтение и десериализация ответа
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

                // Сохранение данных в OutputParameters
                if (responseData != null)
                {
                    foreach (var kvp in responseData)
                    {
                        processNode.OutputParameters = processNode.OutputParameters.Add(kvp.Key, kvp.Value);
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
