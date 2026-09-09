using Account.Application.Common;
using Account.Application.Features.Wallets.Dtos;
using MediatR;

namespace Account.Application.Features.Wallets.Queries.GetMyWallet;

public record GetMyWalletQuery : IRequest<Result<WalletDto>>;
