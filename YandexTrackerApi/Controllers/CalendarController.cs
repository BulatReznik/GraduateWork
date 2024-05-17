using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YandexTrackerApi.BusinessLogic.Models.CalendarModels;

namespace YandexTrackerApi.Controllers
{
    [Route("/api/v1/calendar/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class CalendarController : Controller
    {
        private readonly IMediator _mediator;
        public CalendarController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("/api/v1/calendar/year/")]
        public async Task<IActionResult> InsertIntoCalendar(CalendarYearCreateCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
                return Ok(response.Data);
            return BadRequest(response.ErrorMessage);
        }

        [HttpPost("/api/v1/calendar/day")]
        public async Task<IActionResult> InsertDayIntoCalendar(CalendarDayCreateCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
                return Ok(response.Data);
            return BadRequest(response.ErrorMessage);
        }
    }
}
