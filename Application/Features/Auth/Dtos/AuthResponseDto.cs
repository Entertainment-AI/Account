namespace Account.Application.Features.Auth.Dtos;

public record AuthResponseDto(
    string Token,
    Guid UserId,
    string Email,
    string Username,
    string DisplayName,
    string AvatarUrl,
    string Role,
    bool IsEmailVerified
);