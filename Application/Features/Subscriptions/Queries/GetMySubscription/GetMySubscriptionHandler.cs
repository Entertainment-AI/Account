using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Subscriptions.Dtos;
using Account.Application.Features.Subscriptions.Mappers;
using Account.Domain.Common.DateTimes;
using Account.Domain.Entities;
using Account.Domain.Enums;
using MediatR;

namespace Account.Application.Features.Subscriptions.Queries.GetMySubscription;

public class GetMySubscriptionHandler : IRequestHandler<GetMySubscriptionQuery, Result<UserSubscriptionDto?>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMySubscriptionHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<UserSubscriptionDto?>> Handle(GetMySubscriptionQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        if (currentUserId == null)
        {
            return Result<UserSubscriptionDto?>.Failure(new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        var userId = currentUserId.Value;

        var subRepo = _unitOfWork.GetRepository<UserSubscription>();
        var subs = await subRepo.GetAllAsync(s => s.UserId == userId, cancellationToken);

        // Find currently active subscription, or most recent subscription if none active
        var activeSub = subs
            .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate > Clock.Now)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefault();

        var currentSub = activeSub ?? subs.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
        if (currentSub == null)
        {
            return Result<UserSubscriptionDto?>.Success(null);
        }

        var planRepo = _unitOfWork.GetRepository<Plan>();
        var plan = await planRepo.GetByIdAsync(currentSub.PlanId, cancellationToken);

        return Result<UserSubscriptionDto?>.Success(currentSub.ToUserSubscriptionDto(plan));
    }
}
