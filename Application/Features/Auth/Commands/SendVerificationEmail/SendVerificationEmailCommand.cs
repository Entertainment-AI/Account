using Account.Application.Common;
using MediatR;

namespace Account.Application.Features.Auth.Commands.SendVerificationEmail;

public record SendVerificationEmailCommand(string Email) : IRequest<Result<string>>;