using Account.Domain.Entities;

namespace Account.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    string GenerateEmailVerificationToken(string email);
    (string? Email, bool IsValid) ValidateVerificationToken(string token);
}