using Account.Application.Common;
using Account.Application.Features.Orders.Dtos;
using MediatR;

namespace Account.Application.Features.Orders.Queries.GetMyOrders;

public record GetMyOrdersQuery : IRequest<Result<IReadOnlyList<OrderDto>>>;
