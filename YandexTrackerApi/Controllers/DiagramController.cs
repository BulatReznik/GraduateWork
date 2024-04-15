using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YandexTrackerApi.BusinessLogic.Managers.User;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;
using YandexTrackerApi.BusinessLogic.Models.DiagramQueries;

namespace YandexTrackerApi.Controllers
{
    [Route("/api/v1/diagram/[action]")]
    [ApiController]
    [Authorize]
    public class DiagramController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserManager _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DiagramController(IMediator mediator, IHttpContextAccessor httpContextAccessor, IUserManager userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("/api/v1/diagram/{id:guid}")]
        public async Task<IActionResult> GetDiagram(Guid id)
        {
            var query = new DiagramQuery()
            {
                UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor),
                Id = id
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(query);

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpPost("/api/v1/diagram/")]
        public async Task<IActionResult> PostDiagram(DiagramCreateCommand command)
        {
            command.UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpPost("/api/v1/diagrams/")]
        public async Task<IActionResult> GetDiagrams(DiagramsQuery query)
        {
            query.UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(query);

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpPost("/api/v1/diagrams/update")]
        public async Task<IActionResult> UpdateDiagrams(DiagramUpdateCommand command)
        {
            command.UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpPost("/api/v1/diagrams/execute")]
        public async Task<IActionResult> ExecuteDiagram(DiagramExecuteCommand command)
        {
            command.UserId = _userManager.GetCurrentUserIdByContext(_httpContextAccessor);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }
    }
}
