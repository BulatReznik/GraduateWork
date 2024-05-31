using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System.Text.Json;

namespace BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands
{
    public class GetRequestTaskHandler : ITaskHandler
    {
        private readonly HttpClient _httpClient;

        public GetRequestTaskHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task ExecuteAsync(ProcessNode processNode, ProcessNode previousNode)
        {
            // URL API и параметры запроса
            var apiUrl = processNode.ImportantParameters.FirstOrDefault(ip => ip.Key == "HREF").Value.ToString();

            var requestParameters = new Dictionary<string, string>
            {
                { "parameter1", "value1" },
                { "parameter2", "value2" }
            };

            // Создание запроса
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Content = new FormUrlEncodedContent(requestParameters);

            try
            {
                // Выполнение запроса
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

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
                // Обработка ошибок HTTP-запроса
            }
            catch (Exception e)
            {
                Console.WriteLine($"Общая ошибка: {e.Message}");
                // Обработка других ошибок
            }
        }
    }
}
