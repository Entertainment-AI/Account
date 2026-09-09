using Account.Application.Features.Plans.Commands.CreatePlan;
using Account.Application.Features.Plans.Queries.GetPlanById;
using Account.Application.Features.Plans.Queries.GetPlans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Account.Presentation.Http.Controllers;

[ApiController]
[Route("api/v1/plans")]
public class PlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPlans([FromQuery] bool activeOnly = true)
    {
        var result = await _mediator.Send(new GetPlansQuery(activeOnly));
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPlanById(Guid id)
    {
        var result = await _mediator.Send(new GetPlanByIdQuery(id));
        if (result.IsFailure)
        {
            return NotFound(new { code = result.Error.Code, message = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePlan([FromBody] CreatePlanCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }

        return CreatedAtAction(nameof(GetPlanById), new { id = result.Value!.Id }, result.Value);
    }
}
