using FluentValidation;

namespace Account.Application.Features.Plans.Commands.CreatePlan;

public class CreatePlanValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Plan name is required.")
            .MaximumLength(100).WithMessage("Plan name must not exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Plan code is required.")
            .MaximumLength(50).WithMessage("Plan code must not exceed 50 characters.")
            .Matches(@"^[a-zA-Z0-9_\-]+$").WithMessage("Plan code must only contain letters, digits, dashes, and underscores.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.DurationMonths)
            .GreaterThan(0).WithMessage("Duration must be at least 1 month.");
    }
}
