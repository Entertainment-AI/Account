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
    }
}
