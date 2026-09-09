using Account.Application.Common;
using Account.Application.Features.Plans.Dtos;
using MediatR;

namespace Account.Application.Features.Plans.Queries.GetPlans;

public record GetPlansQuery(bool ActiveOnly = true) : IRequest<Result<IReadOnlyList<PlanDto>>>;
