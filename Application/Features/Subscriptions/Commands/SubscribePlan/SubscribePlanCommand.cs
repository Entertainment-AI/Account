using Account.Application.Common;
using Account.Application.Features.Subscriptions.Dtos;
using Account.Domain.Enums;
using MediatR;

namespace Account.Application.Features.Subscriptions.Commands.SubscribePlan;

public record SubscribePlanCommand(
    Guid PlanId,
    PaymentMethod PaymentMethod = PaymentMethod.Wallet,
    bool AutoRenew = false
) : IRequest<Result<UserSubscriptionDto>>;
