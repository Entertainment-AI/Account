using Account.Application.Common;
using Account.Application.Features.Plans.Dtos;
using MediatR;

namespace Account.Application.Features.Plans.Queries.GetPlanById;

public record GetPlanByIdQuery(Guid Id) : IRequest<Result<PlanDto>>;
