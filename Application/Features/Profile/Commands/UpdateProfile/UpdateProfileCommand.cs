using Account.Application.Common;
using Account.Application.Features.Profile.Dtos;
using MediatR;

namespace Account.Application.Features.Profile.Commands.UpdateProfile;

public record UpdateProfileCommand(Guid UserId, string? DisplayName, string? AvatarUrl) : IRequest<Result<ProfileDto>>;