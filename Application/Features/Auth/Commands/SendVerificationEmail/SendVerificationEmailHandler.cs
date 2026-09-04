using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Domain.Common.DateTimes;
using Account.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Account.Application.Features.Auth.Commands.SendVerificationEmail;

public class SendVerificationEmailCommandHandler : IRequestHandler<SendVerificationEmailCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailSender _emailSender;
    private readonly IMemoryCache _memoryCache;

    public SendVerificationEmailCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        IEmailSender emailSender,
        IMemoryCache memoryCache)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailSender = emailSender;
        _memoryCache = memoryCache;
    }

    public async Task<Result<string>> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var cooldownKey = $"verification_email_cooldown_{normalizedEmail}";

        if (_memoryCache.TryGetValue<DateTime>(cooldownKey, out var nextAllowedAt))
        {
            var remainingSeconds = (int)Math.Max(1, Math.Ceiling((nextAllowedAt - Clock.Now).TotalSeconds));
            return Result<string>.Failure(new Error("RATE_LIMIT_EXCEEDED", $"Please wait {remainingSeconds}s before requesting another verification email."));
        }

        var userRepo = _unitOfWork.GetRepository<User>();
        var user = await userRepo.GetAsync(u => u.Email == normalizedEmail && !u.Deleted, cancellationToken);
        if (user == null)
        {
            return Result<string>.Failure(new Error("USER_NOT_FOUND", "Account not found with email: " + normalizedEmail));
        }

        if (user.IsEmailVerified)
        {
            return Result<string>.Failure(new Error("ALREADY_VERIFIED", "This email is already verified."));
        }

        var token = _jwtTokenGenerator.GenerateEmailVerificationToken(normalizedEmail);
        await _emailSender.SendVerificationLinkAsync(normalizedEmail, token, cancellationToken);

        // Set 60-second cooldown rate limit to prevent email spam
        _memoryCache.Set(cooldownKey, Clock.Now.AddSeconds(60), TimeSpan.FromSeconds(60));

        return Result<string>.Success($"Verification link has been sent to {normalizedEmail}");
    }
}
