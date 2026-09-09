using FluentValidation;

namespace Account.Application.Features.Wallets.Commands.DepositWallet;

public class DepositWalletValidator : AbstractValidator<DepositWalletCommand>
{
    public DepositWalletValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Deposit amount must be greater than 0.");
    }
}
