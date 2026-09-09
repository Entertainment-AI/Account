using Account.Domain.Common;
using Account.Domain.Common.DateTimes;
using Account.Domain.Enums;

namespace Account.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderCode { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public OrderType Type { get; private set; }
    public Guid? PlanId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal FinalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? Metadata { get; private set; }

    // Navigation properties for EF Core
    public User? User { get; private set; }
    public Plan? Plan { get; private set; }

    private Order() { } // EF Core

    private Order(
        string orderCode,
        Guid userId,
        OrderType type,
        decimal amount,
        PaymentMethod paymentMethod,
        Guid? planId = null,
        decimal discountAmount = 0,
        string? metadata = null)
    {
        OrderCode = orderCode.Trim();
        UserId = userId;
        Type = type;
        Amount = amount < 0 ? throw new ArgumentException("Amount cannot be negative.", nameof(amount)) : amount;
        DiscountAmount = discountAmount < 0 ? 0 : discountAmount;
        FinalAmount = Math.Max(0, Amount - DiscountAmount);
        Status = OrderStatus.Pending;
        PaymentMethod = paymentMethod;
        PlanId = planId;
        Metadata = metadata;
    }

    public static Order CreateSubscriptionOrder(
        Guid userId,
        Guid planId,
        decimal amount,
        PaymentMethod paymentMethod,
        decimal discountAmount = 0,
        string? metadata = null)
    {
        var code = GenerateOrderCode("SUB");
        return new Order(code, userId, OrderType.Subscription, amount, paymentMethod, planId, discountAmount, metadata);
    }

    public static Order CreateDepositOrder(
        Guid userId,
        decimal amount,
        PaymentMethod paymentMethod,
        string? metadata = null)
    {
        var code = GenerateOrderCode("DEP");
        return new Order(code, userId, OrderType.WalletDeposit, amount, paymentMethod, null, 0, metadata);
    }

    private static string GenerateOrderCode(string prefix)
    {
        return $"{prefix}_{Clock.Now:yyyyMMdd}_{Guid.CreateVersion7().ToString("N")[..8].ToUpperInvariant()}";
    }

    public void MarkCompleted(DateTime? paidAt = null, string? metadata = null)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException($"Cannot complete order in '{Status}' status.");
        }

        Status = OrderStatus.Completed;
        PaidAt = paidAt ?? Clock.Now;
        if (!string.IsNullOrWhiteSpace(metadata)) Metadata = metadata;
        Touch();
    }

    public void MarkFailed(string? metadata = null)
    {
        if (Status != OrderStatus.Pending) return;

        Status = OrderStatus.Failed;
        if (!string.IsNullOrWhiteSpace(metadata)) Metadata = metadata;
        Touch();
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending) return;

        Status = OrderStatus.Cancelled;
        Touch();
    }
}
