using Account.Domain.Enums;

namespace Account.Application.Features.Profile.Commands.UpdateProfile;

public record UpdateProfileRequest(string? DisplayName, string? AvatarUrl, DateOnly? DateOfBirth, Gender? Gender = null);
