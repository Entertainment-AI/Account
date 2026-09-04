using Account.Application.Features.Auth.Dtos;
using Account.Domain.Entities;

namespace Account.Application.Features.Auth.Mappers;

public static class AuthResponseMapper
{
    public static AuthResponseDto ToAuthResponse(this User user, string token)
    {
        return new AuthResponseDto(
            Token: token,
            UserId: user.Id,
            Email: user.Email,
            Username: user.UserName,
            DisplayName: user.DisplayName ?? string.Empty,
            AvatarUrl: user.AvatarUrl ?? string.Empty,
            Role: user.Role.ToString().ToUpperInvariant(),
            IsEmailVerified: user.IsEmailVerified
        );
    }
}
