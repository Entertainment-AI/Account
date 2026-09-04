using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Profile.Dtos;
using Account.Application.Features.Profile.Mappers;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Profile.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<ProfileDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyProfileQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProfileDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var userRepo = _unitOfWork.GetRepository<User>();
        var user = await userRepo.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null || user.Deleted)
        {
            return Result<ProfileDto>.Failure(new Error("USER_NOT_FOUND", "Profile not found for current user."));
        }

        return Result<ProfileDto>.Success(user.ToProfileDto());
    }
}
