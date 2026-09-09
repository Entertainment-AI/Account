using Account.Application.Common;
using Account.Application.Features.Profile.Dtos;
using Account.Domain.Enums;
using MediatR;

namespace Account.Application.Features.Profile.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string? DisplayName,
    string? AvatarUrl,
    DateOnly? DateOfBirth = null,
    Gender? Gender = null
) : IRequest<Result<ProfileDto>>;