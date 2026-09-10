using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Users.Dtos;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Users.Queries.BatchGetUsers;

public class BatchGetUsersHandler : IRequestHandler<BatchGetUsersQuery, Result<IReadOnlyList<AccountUserDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public BatchGetUsersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<AccountUserDto>>> Handle(BatchGetUsersQuery request, CancellationToken cancellationToken)
    {
        if (request.UserIds == null || request.UserIds.Count == 0)
        {
            return Result<IReadOnlyList<AccountUserDto>>.Success(Array.Empty<AccountUserDto>());
        }

        var distinctIds = request.UserIds.Distinct().ToList();
        var userRepo = _unitOfWork.GetRepository<User>();
        var users = await userRepo.GetAllAsync(u => distinctIds.Contains(u.Id), cancellationToken);

        var dtos = users.Select(user => new AccountUserDto(
            user.Id,
            user.UserName,
            user.DisplayName,
            user.AvatarUrl,
            user.Email,
            user.DateOfBirth,
            user.Gender,
            user.IsEmailVerified
        )).ToList();

        return Result<IReadOnlyList<AccountUserDto>>.Success(dtos);
    }
}
