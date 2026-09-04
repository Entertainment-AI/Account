using Account.Application.Common;
using Account.Application.Features.Auth.Dtos;
using MediatR;

namespace Account.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;