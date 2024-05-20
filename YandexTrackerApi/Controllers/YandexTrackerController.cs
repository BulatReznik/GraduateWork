using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YandexTrackerApi.BusinessLogic.Managers.User;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.BusinessLogic.Models.YandexModels;
using YandexTrackerApi.BusinessLogic.Models.YandexQueries;

namespace YandexTrackerApi.Controllers
{
    [Route("/api/v1/yandex/[action]")]
    [ApiController]
    public class YandexTrackerController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserManager _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public YandexTrackerController(IMediator mediator, IUserManager userManager, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("/api/v1/yandex/project/")]
        [Authorize]
        public async Task<IActionResult> CreateProjectYandexTracker(
            [FromBody] ProjectYandexTrackerCreateCommand command)
        {
            command.UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }


        [HttpPost("/api/v1/yandex/users")]
        public async Task<IActionResult> GetUsersAsync(
            [FromBody] YandexTrackerUsersLoadCommand command)
        {
            command.UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }

        [HttpPost("/api/v1/yandex/user/load/tasks")]
        public async Task<IActionResult> GetUserTaskAsync(
            [FromBody] YandexTrackerIssueByUserByPeriodLoadCommand loadCommand)
        {
            loadCommand.UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(loadCommand);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }

        [HttpPost("/api/v1/yandex/user/tasks")]
        public async Task<IActionResult> GetUserTaskFromDbAsync(
            [FromBody] YandexTrackerIssuesByPeriodQuery query)
        {
            //query.UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor);
            query.UserId = Guid.Parse("01d1f5f1-3baf-45c5-bd98-02108cf13480");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }
    }
}
