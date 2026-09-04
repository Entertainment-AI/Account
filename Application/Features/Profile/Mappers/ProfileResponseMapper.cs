using Account.Application.Features.Profile.Dtos;
using Account.Domain.Entities;

namespace Account.Application.Features.Profile.Mappers;

public static class ProfileResponseMapper
{
    public static ProfileDto ToProfileDto(this User user)
    {
        return new ProfileDto(
            UserId: user.Id,
            Email: user.Email,
            Username: user.UserName,
            DisplayName: user.DisplayName ?? string.Empty,
            AvatarUrl: user.AvatarUrl ?? string.Empty,
            IsEmailVerified: user.IsEmailVerified,
            Role: user.Role.ToString().ToUpperInvariant()
        );
    }
}
