using Account.Application.Common;
using Account.Application.Features.Users.Dtos;
using MediatR;

namespace Account.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<AccountUserDto>>;
