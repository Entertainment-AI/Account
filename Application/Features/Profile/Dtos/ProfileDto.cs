using Account.Domain.Enums;

namespace Account.Application.Features.Profile.Dtos;

public record ProfileDto(
    Guid UserId,
    string Email,
    string Username,
    string DisplayName,
    string AvatarUrl,
    DateOnly? DateOfBirth,
    Gender? Gender,
    bool IsEmailVerified,
    string Role
);