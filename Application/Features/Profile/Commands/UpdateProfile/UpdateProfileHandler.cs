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

    public UpdateProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProfileDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userRepo = _unitOfWork.GetRepository<User>();
        var user = await userRepo.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null || user.Deleted)
        {
            return Result<ProfileDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
        }

        user.UpdateProfile(request.DisplayName, request.AvatarUrl);
        userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProfileDto>.Success(user.ToProfileDto());
    }
}
