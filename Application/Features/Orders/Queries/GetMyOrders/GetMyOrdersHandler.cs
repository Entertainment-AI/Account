using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Orders.Dtos;
using Account.Application.Features.Orders.Mappers;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Orders.Queries.GetMyOrders;

public class GetMyOrdersHandler : IRequestHandler<GetMyOrdersQuery, Result<IReadOnlyList<OrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMyOrdersHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<OrderDto>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        if (currentUserId == null)
        {
            return Result<IReadOnlyList<OrderDto>>.Failure(new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        var userId = currentUserId.Value;

        var orderRepo = _unitOfWork.GetRepository<Order>();
        var orders = await orderRepo.GetAllAsync(o => o.UserId == userId, cancellationToken);

        var dtos = orders
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => o.ToOrderDto())
            .ToList();

        return Result<IReadOnlyList<OrderDto>>.Success(dtos);
    }
}
