using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YandexTrackerApi.BusinessLogic.Models.CalendarModels;
using YandexTrackerApi.BusinessLogic.Models.CalendarModels.CalendarDTO;
using YandexTrackerApi.DbModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace YandexTrackerApi.Controllers
{
    [Route("/api/v1/calendar/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class CalendarController : Controller
    {
        private readonly IMediator _mediator;
        public CalendarController(ILogger logger, IMediator mediator)
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
            else
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
            else
                return BadRequest(response.ErrorMessage);
        }
    }
}
