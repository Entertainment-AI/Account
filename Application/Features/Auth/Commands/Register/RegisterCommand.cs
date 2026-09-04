using Account.Application.Common;
using Account.Application.Features.Auth.Dtos;
using MediatR;

namespace Account.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string? UserName = null, string? DisplayName = null) : IRequest<Result<AuthResponseDto>>;