using Account.Application.Common;
using Account.Application.Features.Wallets.Dtos;
using MediatR;

namespace Account.Application.Features.Wallets.Commands.DepositWallet;

public record DepositWalletCommand(
    decimal Amount,
    string? Note = null
) : IRequest<Result<WalletDto>>;
