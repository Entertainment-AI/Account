namespace Account.Application.Features.Profile.Dtos;

public record ProfileDto(
    Guid UserId,
    string Email,
    string Username,
    string DisplayName,
    string AvatarUrl,
    bool IsEmailVerified,
    string Role
);