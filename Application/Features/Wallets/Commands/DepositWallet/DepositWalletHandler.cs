using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Wallets.Dtos;
using Account.Application.Features.Wallets.Mappers;
using Account.Domain.Common.DateTimes;
using Account.Domain.Entities;
using Account.Domain.Enums;
using MediatR;

namespace Account.Application.Features.Wallets.Commands.DepositWallet;

public class DepositWalletHandler : IRequestHandler<DepositWalletCommand, Result<WalletDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public DepositWalletHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<WalletDto>> Handle(DepositWalletCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        if (currentUserId == null)
        {
            return Result<WalletDto>.Failure(new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        var userId = currentUserId.Value;

        var userRepo = _unitOfWork.GetRepository<User>();
        var user = await userRepo.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.Deleted)
        {
            return Result<WalletDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
        }

        var walletRepo = _unitOfWork.GetRepository<Wallet>();
        var wallet = await walletRepo.GetAsync(w => w.UserId == userId, cancellationToken);
        if (wallet == null)
        {
            wallet = Wallet.Create(userId);
            await walletRepo.AddAsync(wallet, cancellationToken);
        }

        var orderRepo = _unitOfWork.GetRepository<Order>();
        var order = Order.CreateDepositOrder(
            userId: userId,
            amount: request.Amount,
            paymentMethod: PaymentMethod.BankTransfer,
            metadata: request.Note
        );
        order.MarkCompleted(Clock.Now, request.Note ?? "Deposit to wallet");
        await orderRepo.AddAsync(order, cancellationToken);

        wallet.Deposit(request.Amount, request.Note ?? "Deposit to wallet", order.Id);
        walletRepo.Update(wallet);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var txRepo = _unitOfWork.GetRepository<WalletTransaction>();
        var transactions = await txRepo.GetAllAsync(t => t.WalletId == wallet.Id, cancellationToken);

        return Result<WalletDto>.Success(wallet.ToWalletDto(transactions));
    }
}
