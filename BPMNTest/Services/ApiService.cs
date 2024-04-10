using System.Text;
using BPMN.Models;
using Newtonsoft.Json;

namespace BPMN.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseModel<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest requestData)
        {
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseModel = new ResponseModel<TResponse>
                {
                    Data = JsonConvert.DeserializeObject<TResponse>(responseContent)
                };

                return responseModel;
            }

            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                var responseModel = new ResponseModel<TResponse>
                {
                    ErrorMessage = errorMessage
                };

                return responseModel;
            }
        }
    }
}
