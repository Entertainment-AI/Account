using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Users.Dtos;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<AccountUserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AccountUserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userRepo = _unitOfWork.GetRepository<User>();
        var user = await userRepo.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            return Result<AccountUserDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
        }

        var dto = new AccountUserDto(
            user.Id,
            user.UserName,
            user.DisplayName,
            user.AvatarUrl,
            user.Email,
            user.DateOfBirth,
            user.Gender,
            user.IsEmailVerified
        );

        return Result<AccountUserDto>.Success(dto);
    }
}
