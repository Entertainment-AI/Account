using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Wallets.Dtos;
using Account.Application.Features.Wallets.Mappers;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Wallets.Queries.GetMyWallet;

public class GetMyWalletHandler : IRequestHandler<GetMyWalletQuery, Result<WalletDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMyWalletHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<WalletDto>> Handle(GetMyWalletQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        if (currentUserId == null)
        {
            return Result<WalletDto>.Failure(new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        var userId = currentUserId.Value;

        var walletRepo = _unitOfWork.GetRepository<Wallet>();
        var wallet = await walletRepo.GetAsync(w => w.UserId == userId, cancellationToken);

        if (wallet == null)
        {
            wallet = Wallet.Create(userId);
            await walletRepo.AddAsync(wallet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var txRepo = _unitOfWork.GetRepository<WalletTransaction>();
        var transactions = await txRepo.GetAllAsync(t => t.WalletId == wallet.Id, cancellationToken);

        return Result<WalletDto>.Success(wallet.ToWalletDto(transactions));
    }
}
