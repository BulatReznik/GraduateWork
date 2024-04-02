using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YandexTrackerApi.BusinessLogic.Managers.User;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.BusinessLogic.Models.ProjectQueries;

namespace YandexTrackerApi.Controllers
{
    [Route("/api/v1/projects/[action]")]
    [ApiController]
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserManager _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectController(IMediator mediator, IHttpContextAccessor httpContextAccessor, IUserManager userManager)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        [HttpPost("/api/v1/projects/create")]
        public async Task<IActionResult> CreateProject(
            [FromBody] ProjectCreateCommand command)
        {
            command.CreatorId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok() : BadRequest(result.ErrorMessage);
        }

        [HttpGet("/api/v1/projects/")]
        public async Task<IActionResult> GetProjects()
        {
            var query = new ProjectsQuery
            {
                UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor)
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }
    }
}
