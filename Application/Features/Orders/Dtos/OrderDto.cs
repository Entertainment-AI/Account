namespace Account.Application.Features.Orders.Dtos;

public record OrderDto(
    Guid Id,
    string OrderCode,
    Guid UserId,
    string Type,
    Guid? PlanId,
    decimal Amount,
    decimal DiscountAmount,
    decimal FinalAmount,
    string Status,
    string PaymentMethod,
    DateTime? PaidAt,
    string? Metadata,
    DateTime CreatedAt
);
