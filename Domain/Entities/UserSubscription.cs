using Account.Domain.Common;
using Account.Domain.Common.DateTimes;
using Account.Domain.Enums;

namespace Account.Domain.Entities;

public class UserSubscription : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid PlanId { get; private set; }
    public Guid? OrderId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public bool AutoRenew { get; private set; }

    // Navigation properties for EF Core
    public User? User { get; private set; }
    public Plan? Plan { get; private set; }

    private UserSubscription() { } // EF Core

    private UserSubscription(
        Guid userId,
        Guid planId,
        DateTime startDate,
        DateTime endDate,
        Guid? orderId = null,
        SubscriptionStatus status = SubscriptionStatus.Active,
        bool autoRenew = false)
    {
        UserId = userId;
        PlanId = planId;
        OrderId = orderId;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        AutoRenew = autoRenew;
    }

    public static UserSubscription Create(
        Guid userId,
        Guid planId,
        DateTime startDate,
        DateTime endDate,
        Guid? orderId = null,
        SubscriptionStatus status = SubscriptionStatus.Active,
        bool autoRenew = false)
    {
        return new UserSubscription(userId, planId, startDate, endDate, orderId, status, autoRenew);
    }

    public void Extend(int additionalMonths)
    {
        if (additionalMonths <= 0) return;

        var baseDate = EndDate > Clock.Now ? EndDate : Clock.Now;
        EndDate = baseDate.AddMonths(additionalMonths);
        Status = SubscriptionStatus.Active;
        Touch();
    }

    public void Expire()
    {
        Status = SubscriptionStatus.Expired;
        Touch();
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        AutoRenew = false;
        Touch();
    }
}
