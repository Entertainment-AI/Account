using Account.Application.Features.Auth.Commands.Login;
using Account.Application.Features.Auth.Commands.Register;
using Account.Application.Features.Auth.Commands.SendVerificationEmail;
using Account.Application.Features.Auth.Commands.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Account.Presentation.Http.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }
        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }
        return Ok(result.Value);
    }

    [HttpPost("send-verification-email")]
    [HttpPost("send-verification-otp")]
    public async Task<IActionResult> SendVerificationEmail([FromBody] SendVerificationEmailCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }
        return Ok(new { message = result.Value });
    }

    [HttpPost("verify-email")]
    [HttpPost("verify-email-otp")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }
        return Ok(result.Value);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmailGet([FromQuery] string token)
    {
        var result = await _mediator.Send(new VerifyEmailCommand(token));
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }
        return Ok(result.Value);
    }
}