using Account.Application.Features.Orders.Dtos;
using Account.Domain.Entities;

namespace Account.Application.Features.Orders.Mappers;

public static class OrderMapper
{
    public static OrderDto ToOrderDto(this Order order)
    {
        return new OrderDto(
            Id: order.Id,
            OrderCode: order.OrderCode,
            UserId: order.UserId,
            Type: order.Type.ToString().ToUpperInvariant(),
            PlanId: order.PlanId,
            Amount: order.Amount,
            DiscountAmount: order.DiscountAmount,
            FinalAmount: order.FinalAmount,
            Status: order.Status.ToString().ToUpperInvariant(),
            PaymentMethod: order.PaymentMethod.ToString().ToUpperInvariant(),
            PaidAt: order.PaidAt,
            Metadata: order.Metadata,
            CreatedAt: order.CreatedAt
        );
    }
}
