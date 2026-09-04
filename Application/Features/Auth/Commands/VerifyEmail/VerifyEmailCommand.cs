using Account.Application.Common;
using Account.Application.Features.Auth.Dtos;
using MediatR;

namespace Account.Application.Features.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string Token) : IRequest<Result<AuthResponseDto>>;