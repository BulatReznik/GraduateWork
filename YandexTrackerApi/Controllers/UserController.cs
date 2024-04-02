﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YandexTrackerApi.BusinessLogic.Models.TokenModels;
using YandexTrackerApi.BusinessLogic.Models.UserModels;
using YandexTrackerApi.BusinessLogic.Models.UserQueries;

namespace YandexTrackerApi.Controllers
{
    [Route("/api/v1/users/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("/api/v1/users/login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(
            [FromBody] UserLoginQuery query,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(query, token);

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpPost("/api/v1/users/register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            [FromBody] UserRegisterCommand command,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(command, token);
            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpPost("/api/v1/users/token/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenCommand command,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(command, token);
            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }
    }
}