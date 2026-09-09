using FluentValidation;

namespace Account.Application.Features.Subscriptions.Commands.SubscribePlan;

public class SubscribePlanValidator : AbstractValidator<SubscribePlanCommand>
{
    public SubscribePlanValidator()
    {
        RuleFor(x => x.PlanId)
            .NotEmpty().WithMessage("PlanId is required.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method.");
    }
}
