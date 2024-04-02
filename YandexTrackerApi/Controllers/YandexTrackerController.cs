using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using YandexTrackerApi.BusinessLogic.Managers.User;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.BusinessLogic.Queries.YandexQueries;
using YandexTrackerApi.Models.YandexModels;

namespace YandexTest.Controllers
{
    [Route("/api/v1/yandex/[action]")]
    [ApiController]
    public class YandexTrackerController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IUserManager _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public YandexTrackerController(ILogger logger, IMediator mediator, IUserManager userManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("/api/v1/yandex/project/")]
        [Authorize]
        public async Task<IActionResult> AddYandexTracker(
            [FromBody] ProjectYandexTrackerCreateCommand command)
        {
            command.UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }


        [HttpGet("/api/v1/yandex/users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var oauthToken = "y0_AgAAAAAPYtZbAAuAKwAAAAD_UpkdAAAX8Bgeqz9JN64IxV6ZhNbS7k2ZQg";
            var cloudOrgId = "bpfc4r4fii0i9kvqavto";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {oauthToken}");
            httpClient.DefaultRequestHeaders.Add("X-Cloud-Org-ID", cloudOrgId);

            var apiUrl = "https://api.tracker.yandex.net/";
            var endpoint = "v2/users/";

            var response = await httpClient.GetAsync(apiUrl + endpoint);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<User>>(responseBody);
                return Ok(users);
            }
            else
            {
                string message = $"Не удалось получить данные. Код состояния: {response.StatusCode}";
                _logger.LogError(message);
                return BadRequest(message);
            }
        }

        [HttpPost("/api/v1/yandex/count/backlog")]
        public async Task<IActionResult> CountTaskBackLogAsync()
        {
            var oauthToken = "y0_AgAAAAAPYtZbAAuAKwAAAAD_UpkdAAAX8Bgeqz9JN64IxV6ZhNbS7k2ZQg";
            var cloudOrgId = "bpfc4r4fii0i9kvqavto";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {oauthToken}");
            httpClient.DefaultRequestHeaders.Add("X-Cloud-Org-ID", cloudOrgId);

            var apiUrl = "https://api.tracker.yandex.net/";
            var endpoint = "v2/issues/_count/";

            var filter = new
            {
                queue = "BEKLOG"
            };

            var content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(apiUrl + endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var count = JsonConvert.DeserializeObject<int>(responseBody);
                return Ok(count);
            }
            else
            {
                string message = $"Не удалось получить данные. Код состояния: {response.StatusCode}";
                _logger.LogError(message);
                return BadRequest(message);
            }
        }

        [HttpPost("/api/v1/yandex/myself/task/backlog")]
        public async Task<IActionResult> TaskBackLogMyselfAsync(IssuesByPeriodQuery query)
        {
            var oauthToken = "y0_AgAAAAAPYtZbAAuAKwAAAAD_UpkdAAAX8Bgeqz9JN64IxV6ZhNbS7k2ZQg";
            var cloudOrgId = "bpfc4r4fii0i9kvqavto";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {oauthToken}");
            httpClient.DefaultRequestHeaders.Add("X-Cloud-Org-ID", cloudOrgId);

            var apiUrl = "https://api.tracker.yandex.net/";
            var endpoint = "v2/issues/_search";

            query = new();

            var filter = new
            {
                query = $"Assignee: {query.Assignee} Queue: BACKDEV \"Start Date\": \"{query.StartDate:yyyy-MM-dd}\"..\"{query.EndDate:yyyy-MM-dd}\""
            };


            var content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(apiUrl + endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var issues = JsonConvert.DeserializeObject<List<Issue>>(responseBody);

                if (issues != null)
                {
                    foreach (var issue in issues)
                    {
                        if (issue.OriginalEstimation != null)
                        {
                            _logger.LogInformation(message: issue.OriginalEstimation);
                        }
                    }

                    double totalSpentTime = GetTotalSpentTime(issues);
                    _logger.LogInformation($"Общее потраченное время: {totalSpentTime} часов");
                }

                return Ok(issues);
            }
            else
            {
                string message = $"Не удалось получить данные. Код состояния: {response.StatusCode}";
                _logger.LogError(message);
                return BadRequest(message);
            }
        }

        private double GetTotalSpentTime(List<Issue> issues)
        {
            double totalSpentTime = 0;

            foreach (var issue in issues)
            {
                var spentTimeString = issue.Spent;

                if (spentTimeString != null)
                {
                    if (spentTimeString.StartsWith("P"))
                    {
                        int days = 0;
                        int hours = 0;

                        var durationParts = spentTimeString.Split('T');

                        if (durationParts.Length == 2)
                        {
                            var daysString = durationParts[0].Replace("P", "");
                            var hoursString = durationParts[1];

                            if (daysString.Contains('D'))
                            {
                                daysString = daysString.Replace("D", "");
                                int.TryParse(daysString, out days);
                            }

                            if (hoursString.Contains('H'))
                            {
                                hoursString = hoursString.Replace("H", "");
                                int.TryParse(hoursString, out hours);
                            }
                        }

                        totalSpentTime += days * 8 + hours;
                        _logger.LogInformation(totalSpentTime.ToString());
                    }
                    else
                    {
                        _logger.LogError("Неверный формат строки времени.");
                        // Обработка неверного формата строки времени, например, выбрасывание исключения или установка значения по умолчанию
                    }
                }
            }

            return totalSpentTime;
        }

        [HttpGet("/api/v1/yandex/myself")]
        public async Task<IActionResult> GetMyselfAsync()
        {
            var oauthToken = "y0_AgAAAAAPYtZbAAuAKwAAAAD_UpkdAAAX8Bgeqz9JN64IxV6ZhNbS7k2ZQg";
            var cloudOrgId = "bpfc4r4fii0i9kvqavto";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {oauthToken}");
            httpClient.DefaultRequestHeaders.Add("X-Cloud-Org-ID", cloudOrgId);

            var apiUrl = "https://api.tracker.yandex.net/";
            var endpoint = "v2/myself/";

            var response = await httpClient.GetAsync(apiUrl + endpoint);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var myself = JsonConvert.DeserializeObject<User>(responseBody);
                return Ok(myself);
            }
            else
            {
                string message = $"Не удалось получить данные. Код состояния: {response.StatusCode}";
                _logger.LogError(message);
                return BadRequest(message);
            }
        }
    }
}
