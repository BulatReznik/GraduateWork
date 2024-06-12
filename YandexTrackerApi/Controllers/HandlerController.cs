using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YandexTrackerApi.BusinessLogic.Models.DiagramQueries;
using MediatR;
using YandexTrackerApi.BusinessLogic.Managers.User;
using Microsoft.AspNetCore.Identity;
using YandexTrackerApi.BusinessLogic.Models.ProjectQueries;
using YandexTrackerApi.BusinessLogic.Models.HandlersQueries;

namespace YandexTrackerApi.Controllers
{

    [Route("/api/v1/handler/[action]")]
    [ApiController]
    public class HandlerController : Controller
    {

        private readonly IMediator _mediator;

        public HandlerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/api/v1/handlers/")]
        public async Task<IActionResult> GetHandlers()
        {
            var query = new HandlersQuery{};

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(query);

            if (response.IsSuccess)
                return Ok(response.Data);
            return BadRequest(response.ErrorMessage);
        }
    }
}
