using System.Text.RegularExpressions;
using Account.Domain.Common;
using Account.Domain.Common.DateTimes;
using Account.Domain.Enums;

namespace Account.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string UserName { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string AvatarUrl { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public DateTime? LastUserNameChangedAt { get; private set; }

    private User() { } // EF Core

    private User(
        string email,
        string passwordHash,
        string userName,
        string? displayName = null,
        string? avatarUrl = null,
        UserRole role = UserRole.User,
        bool isEmailVerified = false)
    {
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        UserName = NormalizeUserName(userName);
        DisplayName = displayName?.Trim() ?? (userName?.Trim() ?? string.Empty);
        AvatarUrl = avatarUrl?.Trim() ?? string.Empty;
        Role = role;
        IsEmailVerified = isEmailVerified;
    }

    public static User Create(
        string email,
        string passwordHash,
        string userName,
        string? displayName = null,
        string? avatarUrl = null,
        UserRole role = UserRole.User,
        bool isEmailVerified = false)
    {
        return new User(email, passwordHash, userName, displayName, avatarUrl, role, isEmailVerified);
    }

    private string NormalizeUserName(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "user_" + Guid.CreateVersion7().ToString("N")[..6];
        var cleaned = Regex.Replace(raw.Trim().ToLowerInvariant(), @"[^a-z0-9_]", "");
        if (cleaned.Length < 3) cleaned = cleaned + "_" + Guid.CreateVersion7().ToString("N")[..4];
        if (cleaned.Length > 30) cleaned = cleaned[..30];
        return cleaned;
    }

    public void UpdateProfile(string? displayName, string? avatarUrl)
    {
        if (!string.IsNullOrWhiteSpace(displayName)) DisplayName = displayName.Trim();
        if (avatarUrl != null) AvatarUrl = avatarUrl.Trim();
        Touch();
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        Touch();
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        Touch();
    }
}
