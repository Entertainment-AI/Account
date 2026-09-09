using Account.Domain.Common.DateTimes;
using FluentValidation;

namespace Account.Application.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.DisplayName)
            .MaximumLength(50).WithMessage("Display name cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.DisplayName));

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(Clock.Now))
            .WithMessage("Date of birth cannot be in the future.")
            .GreaterThan(_ => DateOnly.FromDateTime(Clock.Now.AddYears(-120)))
            .WithMessage("Date of birth must be within the last 120 years.")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value.")
            .When(x => x.Gender.HasValue);
    }
}
