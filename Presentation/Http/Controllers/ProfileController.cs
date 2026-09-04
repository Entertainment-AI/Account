using System.Security.Claims;
using Account.Application.Features.Profile.Commands.UpdateProfile;
using Account.Application.Features.Profile.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Account.Presentation.Http.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public record UpdateProfileRequest(string? DisplayName, string? AvatarUrl);

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdStr = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORIZED", message = "User is not authenticated." });
        }

        var result = await _mediator.Send(new GetMyProfileQuery(userId));
        if (result.IsFailure)
        {
            return NotFound(new { code = result.Error.Code, message = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
    {
        var userIdStr = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORIZED", message = "User is not authenticated." });
        }

        var result = await _mediator.Send(new UpdateProfileCommand(userId, request.DisplayName, request.AvatarUrl));
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }

        return Ok(result.Value);
    }
}