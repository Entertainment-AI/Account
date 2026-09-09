using Account.Application.Features.Wallets.Dtos;
using Account.Domain.Entities;

namespace Account.Application.Features.Wallets.Mappers;

public static class WalletMapper
{
    public static WalletDto ToWalletDto(this Wallet wallet, IEnumerable<WalletTransaction>? transactions = null)
    {
        var txList = (transactions ?? wallet.Transactions ?? [])
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => t.ToTransactionDto())
            .ToList();

        return new WalletDto(
            Id: wallet.Id,
            UserId: wallet.UserId,
            Balance: wallet.Balance,
            Currency: wallet.Currency,
            Status: wallet.Status.ToString().ToUpperInvariant(),
            RecentTransactions: txList
        );
    }

    public static WalletTransactionDto ToTransactionDto(this WalletTransaction tx)
    {
        return new WalletTransactionDto(
            Id: tx.Id,
            TransactionCode: tx.TransactionCode,
            Amount: tx.Amount,
            BalanceBefore: tx.BalanceBefore,
            BalanceAfter: tx.BalanceAfter,
            Type: tx.Type.ToString().ToUpperInvariant(),
            Description: tx.Description,
            OrderId: tx.OrderId,
            CreatedAt: tx.CreatedAt
        );
    }
}
