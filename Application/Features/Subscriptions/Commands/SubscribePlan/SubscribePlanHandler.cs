using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Subscriptions.Dtos;
using Account.Application.Features.Subscriptions.Mappers;
using Account.Domain.Common.DateTimes;
using Account.Domain.Entities;
using Account.Domain.Enums;
using MediatR;

namespace Account.Application.Features.Subscriptions.Commands.SubscribePlan;

public class SubscribePlanHandler : IRequestHandler<SubscribePlanCommand, Result<UserSubscriptionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public SubscribePlanHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<UserSubscriptionDto>> Handle(SubscribePlanCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        if (currentUserId == null)
        {
            return Result<UserSubscriptionDto>.Failure(new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        var userId = currentUserId.Value;

        var userRepo = _unitOfWork.GetRepository<User>();
        var user = await userRepo.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.Deleted)
        {
            return Result<UserSubscriptionDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
        }

        var planRepo = _unitOfWork.GetRepository<Plan>();
        var plan = await planRepo.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null || !plan.IsActive || plan.Deleted)
        {
            return Result<UserSubscriptionDto>.Failure(new Error("PLAN_NOT_FOUND", "Plan not found or inactive."));
        }

        var orderRepo = _unitOfWork.GetRepository<Order>();
        var walletRepo = _unitOfWork.GetRepository<Wallet>();
        var subRepo = _unitOfWork.GetRepository<UserSubscription>();

        Order order;

        if (plan.Price == 0)
        {
            order = Order.CreateSubscriptionOrder(user.Id, plan.Id, 0, PaymentMethod.Free);
            order.MarkCompleted(Clock.Now, $"Free plan subscription: {plan.Name}");
            await orderRepo.AddAsync(order, cancellationToken);
        }
        else
        {
            if (request.PaymentMethod == PaymentMethod.Wallet)
            {
                var wallet = await walletRepo.GetAsync(w => w.UserId == user.Id, cancellationToken);
                if (wallet == null)
                {
                    wallet = Wallet.Create(user.Id);
                    await walletRepo.AddAsync(wallet, cancellationToken);
                }

                if (wallet.Balance < plan.Price)
                {
                    return Result<UserSubscriptionDto>.Failure(new Error(
                        "INSUFFICIENT_BALANCE",
                        $"Insufficient wallet balance. Current balance: {wallet.Balance:N0} {wallet.Currency}, Required: {plan.Price:N0} {wallet.Currency}."
                    ));
                }

                order = Order.CreateSubscriptionOrder(user.Id, plan.Id, plan.Price, PaymentMethod.Wallet);
                order.MarkCompleted(Clock.Now, $"Subscription payment for plan: {plan.Name}");
                await orderRepo.AddAsync(order, cancellationToken);

                wallet.Deduct(plan.Price, $"Payment for plan: {plan.Name}", order.Id);
                walletRepo.Update(wallet);
            }
            else
            {
                return Result<UserSubscriptionDto>.Failure(new Error(
                    "UNSUPPORTED_PAYMENT_METHOD",
                    "Currently, only Wallet payment is supported for instant subscription. Please top up your wallet or select a Free plan."
                ));
            }
        }

        // Check if user already has an active subscription
        var existingSubs = await subRepo.GetAllAsync(s => s.UserId == user.Id, cancellationToken);
        var activeSub = existingSubs
            .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate > Clock.Now)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefault();

        UserSubscription subscription;
        if (activeSub != null)
        {
            if (activeSub.PlanId == plan.Id)
            {
                // Extend the current subscription
                activeSub.Extend(plan.DurationMonths);
                subRepo.Update(activeSub);
                subscription = activeSub;
            }
            else
            {
                // Expire existing plan subscription and start new one
                activeSub.Expire();
                subRepo.Update(activeSub);

                subscription = UserSubscription.Create(
                    userId: user.Id,
                    planId: plan.Id,
                    startDate: Clock.Now,
                    endDate: Clock.Now.AddMonths(plan.DurationMonths),
                    orderId: order.Id,
                    status: SubscriptionStatus.Active,
                    autoRenew: request.AutoRenew
                );
                await subRepo.AddAsync(subscription, cancellationToken);
            }
        }
        else
        {
            subscription = UserSubscription.Create(
                userId: user.Id,
                planId: plan.Id,
                startDate: Clock.Now,
                endDate: Clock.Now.AddMonths(plan.DurationMonths),
                orderId: order.Id,
                status: SubscriptionStatus.Active,
                autoRenew: request.AutoRenew
            );
            await subRepo.AddAsync(subscription, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserSubscriptionDto>.Success(subscription.ToUserSubscriptionDto(plan));
    }
}
