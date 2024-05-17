using System.Text;
using BPMN.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace BPMN.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpContext _httpContext;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<ResponseModel<TResponse>> GetAsync<TResponse>(string url)
        {
            var accessToken = _httpContext.Session.GetString("AccessToken");

            // Добавим токены в заголовки запроса, если они предоставлены
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseModel = new ResponseModel<TResponse>
                {
                    Data = JsonConvert.DeserializeObject<TResponse>(responseContent)
                };

                return responseModel;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var responseModel = new ResponseModel<TResponse>
                {
                    ErrorMessage = "Пользователь не авторизован"
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

        public async Task<ResponseModel<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest requestData)
        {
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var accessToken = _httpContext.Session.GetString("AccessToken");

            //Добавим токены в заголовки запроса, если они предоставлены
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

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
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                var responseModel = new ResponseModel<TResponse>
                {
                    ErrorMessage = errorMessage
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

        public async Task<ResponseModel<string>> PostStringAsync<TRequest>(string url, TRequest requestData)
        {
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var accessToken = _httpContext.Session.GetString("AccessToken");

            //Добавим токены в заголовки запроса, если они предоставлены
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseModel = new ResponseModel<string>
                {
                    Data = responseContent
                };

                return responseModel;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                var responseModel = new ResponseModel<string>
                {
                    ErrorMessage = errorMessage
                };

                return responseModel;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                var responseModel = new ResponseModel<string>
                {
                    ErrorMessage = errorMessage
                };

                return responseModel;
            }
        }
    }
}
