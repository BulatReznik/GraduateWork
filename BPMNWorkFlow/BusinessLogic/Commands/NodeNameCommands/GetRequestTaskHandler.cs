using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    public class GetRequestTaskHandler : ITaskHandler
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

            // Создание HttpClient
            using var httpClient = new HttpClient();

            // Создание запроса
            var requestUri = new Uri(QueryHelpers.AddQueryString(apiUrl, requestParameters));
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            try
            {
                // Выполнение запроса
                var response = await httpClient.SendAsync(request);

                // Чтение и десериализация ответа
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);

                // Сохранение данных в OutputParameters
                if (responseData != null)
                {
                    foreach (var kvp in responseData)
                    {
                        processNode.OutputParameters.Add(kvp.Key, kvp.Value);
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
