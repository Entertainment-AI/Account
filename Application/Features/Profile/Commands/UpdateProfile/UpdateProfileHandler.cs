using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Profile.Dtos;
using Account.Application.Features.Profile.Mappers;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<ProfileDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateProfileCommandHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<ProfileDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        if (currentUserId == null)
        {
            return Result<ProfileDto>.Failure(new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        var userId = currentUserId.Value;

        var userRepo = _unitOfWork.GetRepository<User>();
        var user = await userRepo.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.Deleted)
        {
            return Result<ProfileDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
        }

        user.UpdateProfile(request.DisplayName, request.AvatarUrl, request.DateOfBirth, request.Gender);
        userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProfileDto>.Success(user.ToProfileDto());
    }
}
