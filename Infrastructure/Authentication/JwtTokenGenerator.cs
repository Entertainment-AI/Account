using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Account.Application.Common.Interfaces;
using Account.Domain.Common.DateTimes;
using Account.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Account.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenGenerator> _logger;

    public JwtTokenGenerator(IConfiguration configuration, ILogger<JwtTokenGenerator> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string GetRequiredConfig(string key)
    {
        var value = _configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            _logger.LogError("Configuration key '{Key}' is null, missing, or empty.", key);
            throw new InvalidOperationException($"Configuration key '{key}' is null, missing, or empty.");
        }
        return value;
    }

    private string Secret => GetRequiredConfig("Jwt:Secret");
    private string Issuer => GetRequiredConfig("Jwt:Issuer");
    private string Audience => GetRequiredConfig("Jwt:Audience");

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.InboundClaimTypeMap.Clear();
        tokenHandler.OutboundClaimTypeMap.Clear();
        var key = Encoding.UTF8.GetBytes(Secret);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new("userId", user.Id.ToString()),
            new("username", user.UserName),
            new("displayName", user.DisplayName ?? string.Empty),
            new("avatarUrl", user.AvatarUrl ?? string.Empty),
            new("role", user.Role.ToString().ToUpperInvariant()),
            new("isEmailVerified", user.IsEmailVerified ? "true" : "false"),
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString())
        };

        if (!double.TryParse(_configuration["Jwt:ExpirationMinutes"], out var expirationMinutes) || expirationMinutes <= 0)
        {
            _logger.LogError("Configuration key 'Jwt:ExpirationMinutes' is null, missing, or invalid.");
            throw new InvalidOperationException("Configuration key 'Jwt:ExpirationMinutes' is null, missing, or invalid.");
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = Clock.Now.AddMinutes(expirationMinutes),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateEmailVerificationToken(string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.InboundClaimTypeMap.Clear();
        tokenHandler.OutboundClaimTypeMap.Clear();
        var key = Encoding.UTF8.GetBytes(Secret);

        var claims = new List<Claim>
        {
            new("purpose", "email_verification"),
            new("email", email.Trim().ToLowerInvariant()),
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = Clock.Now.AddHours(24), // 24 hours
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public (string? Email, bool IsValid) ValidateVerificationToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.InboundClaimTypeMap.Clear();
            var key = Encoding.UTF8.GetBytes(Secret);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);
            var purpose = principal.FindFirst("purpose")?.Value;
            var email = principal.FindFirst("email")?.Value 
                     ?? principal.FindFirst(ClaimTypes.Email)?.Value
                     ?? principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (purpose == "email_verification" && !string.IsNullOrWhiteSpace(email))
            {
                return (email, true);
            }

            _logger.LogWarning("Token validation missing purpose or email claim. Purpose: {Purpose}, Email: {Email}", purpose, email);
            return (null, false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation exception: {Message}", ex.Message);
            return (null, false);
        }
    }
}
