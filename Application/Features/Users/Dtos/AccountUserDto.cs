using Account.Domain.Enums;

namespace Account.Application.Features.Users.Dtos;

public record AccountUserDto(
    Guid UserId,
    string? UserName,
    string? DisplayName,
    string? AvatarUrl,
    string? Email = null,
    DateOnly? DateOfBirth = null,
    Gender? Gender = null,
    bool IsEmailVerified = false
);

public record BatchGetUsersRequest(
    List<Guid> UserIds
);

