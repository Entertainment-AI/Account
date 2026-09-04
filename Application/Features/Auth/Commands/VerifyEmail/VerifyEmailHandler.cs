using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Auth.Dtos;
using Account.Application.Features.Auth.Mappers;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public VerifyEmailCommandHandler(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponseDto>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var (email, isValid) = _jwtTokenGenerator.ValidateVerificationToken(request.Token);
        if (!isValid || string.IsNullOrWhiteSpace(email))
        {
            return Result<AuthResponseDto>.Failure(new Error("INVALID_TOKEN", "Verification link is invalid or has expired."));
        }

        var userRepo = _unitOfWork.GetRepository<User>();
        var user = await userRepo.GetAsync(u => u.Email == email.Trim().ToLowerInvariant() && !u.Deleted, cancellationToken);
        if (user == null)
        {
            return Result<AuthResponseDto>.Failure(new Error("USER_NOT_FOUND", "Account not found for this verification link."));
        }

        user.VerifyEmail();
        userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var newToken = _jwtTokenGenerator.GenerateToken(user);
        return Result<AuthResponseDto>.Success(user.ToAuthResponse(newToken));
    }
}
