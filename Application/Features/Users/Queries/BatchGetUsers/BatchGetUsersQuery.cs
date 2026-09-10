using Account.Application.Common;
using Account.Application.Features.Users.Dtos;
using MediatR;

namespace Account.Application.Features.Users.Queries.BatchGetUsers;

public record BatchGetUsersQuery(IReadOnlyList<Guid> UserIds) : IRequest<Result<IReadOnlyList<AccountUserDto>>>;
