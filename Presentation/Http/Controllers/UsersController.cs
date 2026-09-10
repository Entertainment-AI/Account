using Account.Application.Features.Users.Dtos;
using Account.Application.Features.Users.Queries.BatchGetUsers;
using Account.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Account.Presentation.Http.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        if (result.IsFailure)
        {
            return NotFound(new { code = result.Error.Code, message = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost("batch")]
    [AllowAnonymous]
    public async Task<IActionResult> BatchGetUsers([FromBody] BatchGetUsersRequest request)
    {
        if (request == null || request.UserIds == null)
        {
            return Ok(Array.Empty<AccountUserDto>());
        }

        var result = await _mediator.Send(new BatchGetUsersQuery(request.UserIds));
        if (result.IsFailure)
        {
            return BadRequest(new { code = result.Error.Code, message = result.Error.Message });
        }

        return Ok(result.Value);
    }
}
