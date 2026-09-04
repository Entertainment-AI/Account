using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Auth.Commands.SendVerificationEmail;

public class SendVerificationEmailCommandHandler : IRequestHandler<SendVerificationEmailCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailSender _emailSender;

    public SendVerificationEmailCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailSender = emailSender;
    }

    public async Task<Result<string>> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
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

        return Result<string>.Success($"Verification link has been sent to {normalizedEmail}");
    }
}
