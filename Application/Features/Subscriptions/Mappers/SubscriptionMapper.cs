using Account.Application.Features.Subscriptions.Dtos;
using Account.Domain.Common.DateTimes;
using Account.Domain.Entities;
using Account.Domain.Enums;

namespace Account.Application.Features.Subscriptions.Mappers;

public static class SubscriptionMapper
{
    public static UserSubscriptionDto ToUserSubscriptionDto(this UserSubscription subscription, Plan? plan = null)
    {
        var isActive = subscription.Status == SubscriptionStatus.Active && subscription.EndDate > Clock.Now;
        return new UserSubscriptionDto(
            Id: subscription.Id,
            UserId: subscription.UserId,
            PlanId: subscription.PlanId,
            PlanName: plan?.Name ?? "Unknown Plan",
            PlanCode: plan?.Code ?? "UNKNOWN",
            PlanPrice: plan?.Price ?? 0,
            DurationMonths: plan?.DurationMonths ?? 0,
            OrderId: subscription.OrderId,
            StartDate: subscription.StartDate,
            EndDate: subscription.EndDate,
            Status: subscription.Status,
            AutoRenew: subscription.AutoRenew,
            IsActive: isActive
        );
    }
}
