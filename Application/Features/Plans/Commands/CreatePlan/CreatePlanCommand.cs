using Account.Application.Common;
using Account.Application.Features.Plans.Dtos;
using MediatR;

namespace Account.Application.Features.Plans.Commands.CreatePlan;

public record CreatePlanCommand(
    string Name,
    string Code,
    decimal Price,
    int DurationMonths,
    string? Description = null,
    string? Features = null,
    bool IsActive = true
) : IRequest<Result<PlanDto>>;
