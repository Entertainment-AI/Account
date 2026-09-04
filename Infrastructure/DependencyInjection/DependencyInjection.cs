using System.Text;
using Account.Application.Common.Interfaces;
using Account.Infrastructure.Authentication;
using Account.Infrastructure.Email;
using Account.Infrastructure.Persistence.Context;
using Account.Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Account.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("IdentityDb")
            ?? throw new InvalidOperationException("DefaultConnection or IdentityDb string is not configured.");

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        services.AddDbContext<AccountDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IAccountDbContext>(provider => provider.GetRequiredService<AccountDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IEmailSender, EmailSenderService>();

        var jwtSecret = configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(jwtSecret))
            throw new InvalidOperationException("Configuration key 'Jwt:Secret' is null, missing, or empty.");

        var jwtIssuer = configuration["Jwt:Issuer"];
        if (string.IsNullOrWhiteSpace(jwtIssuer))
            throw new InvalidOperationException("Configuration key 'Jwt:Issuer' is null, missing, or empty.");

        var jwtAudience = configuration["Jwt:Audience"];
        if (string.IsNullOrWhiteSpace(jwtAudience))
            throw new InvalidOperationException("Configuration key 'Jwt:Audience' is null, missing, or empty.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        });

        services.AddAuthorization();

        return services;
    }
}
