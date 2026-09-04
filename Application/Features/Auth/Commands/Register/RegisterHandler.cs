using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Auth.Dtos;
using Account.Application.Features.Auth.Mappers;
using Account.Domain.Entities;
using Account.Domain.Enums;
using MediatR;

namespace Account.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var userRepo = _unitOfWork.GetRepository<User>();

        var existingUser = await userRepo.GetAsync(u => u.Email == normalizedEmail && !u.Deleted, cancellationToken);
        if (existingUser != null)
        {
            return Result<AuthResponseDto>.Failure(new Error("EMAIL_ALREADY_EXISTS", "This email address is already registered."));
        }

        var rawUserName = string.IsNullOrWhiteSpace(request.UserName)
            ? normalizedEmail.Split('@')[0]
            : request.UserName.Trim();

        var existingUserName = await userRepo.GetAsync(u => u.UserName == rawUserName && !u.Deleted, cancellationToken);
        if (existingUserName != null)
        {
            rawUserName = $"{rawUserName}_{Guid.CreateVersion7().ToString("N")[..4]}";
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var displayName = string.IsNullOrWhiteSpace(request.DisplayName) ? rawUserName : request.DisplayName.Trim();

        var user = User.Create(
            email: normalizedEmail,
            passwordHash: passwordHash,
            userName: rawUserName,
            displayName: displayName,
            role: UserRole.User
        );

        await userRepo.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);
        return Result<AuthResponseDto>.Success(user.ToAuthResponse(token));
    }
}
