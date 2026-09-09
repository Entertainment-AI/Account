using Account.Application.Features.Subscriptions.Commands.SubscribePlan;
using Account.Application.Features.Subscriptions.Queries.GetMySubscription;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Account.Presentation.Http.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
[Authorize]
public class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMySubscription()
    {
        var result = await _mediator.Send(new GetMySubscriptionQuery());
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }

        if (result.Value == null)
        {
            return Ok(new { message = "No active subscription found.", subscription = (object?)null });
        }

        return Ok(result.Value);
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> SubscribePlan([FromBody] SubscribePlanCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }

        return Ok(result.Value);
    }
}
