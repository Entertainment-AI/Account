namespace Account.Application.Features.Plans.Dtos;

public record PlanDto(
    Guid Id,
    string Name,
    string Code,
    decimal Price,
    int DurationMonths,
    string? Description,
    string? Features,
    bool IsActive
);
