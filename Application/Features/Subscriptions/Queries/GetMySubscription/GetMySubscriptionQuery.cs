using Account.Application.Common;
using Account.Application.Features.Subscriptions.Dtos;
using MediatR;

namespace Account.Application.Features.Subscriptions.Queries.GetMySubscription;

public record GetMySubscriptionQuery : IRequest<Result<UserSubscriptionDto?>>;
