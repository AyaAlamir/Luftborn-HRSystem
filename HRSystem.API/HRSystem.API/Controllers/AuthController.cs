using System;
using System.Threading.Tasks;
using HRSystem.Application.Auth;
using HRSystem.Application.Auth.Commands;
using HRSystem.Application.Common;
using HRSystem.Application.Localization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _localizer;

        public AuthController(IMediator mediator, ILocalizationService localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(_localizer.Get("ValidationFailed")));
            }

            var response = await _mediator.Send(new LoginCommand(request));
            if (response == null)
            {
                return Unauthorized(ApiResponse<object>.Fail(_localizer.Get("InvalidLogin")));
            }

            return Ok(ApiResponse<AuthResponse>.Ok(response, _localizer.Get("LoginSuccess")));
        }

        [HttpPost("google")]
        public async Task<IActionResult> Google(GoogleLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(_localizer.Get("ValidationFailed")));
            }

            try
            {
                var response = await _mediator.Send(new GoogleLoginCommand(request));
                if (response == null)
                {
                    return Unauthorized(ApiResponse<object>.Fail(_localizer.Get("GoogleLoginFailed")));
                }

                return Ok(ApiResponse<AuthResponse>.Ok(response, _localizer.Get("LoginSuccess")));
            }
            catch (Exception)
            {
                return Unauthorized(ApiResponse<object>.Fail(_localizer.Get("GoogleLoginFailed")));
            }
        }
    }
}
