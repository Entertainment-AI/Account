using Account.Domain.Enums;

namespace Account.Application.Features.Subscriptions.Dtos;

public record UserSubscriptionDto(
    Guid Id,
    Guid UserId,
    Guid PlanId,
    string PlanName,
    string PlanCode,
    decimal PlanPrice,
    int DurationMonths,
    Guid? OrderId,
    DateTime StartDate,
    DateTime EndDate,
    SubscriptionStatus Status,
    bool AutoRenew,
    bool IsActive
);
