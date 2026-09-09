using Account.Application.Features.Plans.Dtos;
using Account.Domain.Entities;

namespace Account.Application.Features.Plans.Mappers;

public static class PlanMapper
{
    public static PlanDto ToPlanDto(this Plan plan)
    {
        return new PlanDto(
            Id: plan.Id,
            Name: plan.Name,
            Code: plan.Code,
            Price: plan.Price,
            DurationMonths: plan.DurationMonths,
            Description: plan.Description,
            Features: plan.Features,
            IsActive: plan.IsActive
        );
    }
}
