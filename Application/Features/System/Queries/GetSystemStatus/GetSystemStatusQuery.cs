using Account.Application.Common;
using Account.Application.Features.System.Dtos;
using MediatR;

namespace Account.Application.Features.System.Queries.GetSystemStatus;

public record GetSystemStatusQuery : IRequest<Result<SystemStatusDto>>;
