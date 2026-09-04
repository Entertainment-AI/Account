using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Auth.Dtos;
using Account.Application.Features.Auth.Mappers;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var userRepo = _unitOfWork.GetRepository<User>();

        var user = await userRepo.GetAsync(u => u.Email == normalizedEmail && !u.Deleted, cancellationToken);
        if (user == null)
        {
            return Result<AuthResponseDto>.Failure(new Error("INVALID_CREDENTIALS", "Invalid email or password."));
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponseDto>.Failure(new Error("INVALID_CREDENTIALS", "Invalid email or password."));
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return Result<AuthResponseDto>.Success(user.ToAuthResponse(token));
    }
}
