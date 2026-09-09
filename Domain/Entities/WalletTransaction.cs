using Account.Domain.Common;
using Account.Domain.Common.DateTimes;
using Account.Domain.Enums;

namespace Account.Domain.Entities;

public class WalletTransaction : BaseEntity
{
    public string TransactionCode { get; private set; } = null!;
    public Guid WalletId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal BalanceBefore { get; private set; }
    public decimal BalanceAfter { get; private set; }
    public WalletTransactionType Type { get; private set; }
    public Guid? OrderId { get; private set; }
    public string Description { get; private set; } = null!;

    private WalletTransaction() { } // EF Core

    internal WalletTransaction(
        Guid walletId,
        decimal amount,
        decimal balanceBefore,
        decimal balanceAfter,
        WalletTransactionType type,
        string description,
        Guid? orderId = null,
        string? transactionCode = null)
    {
        WalletId = walletId;
        Amount = amount;
        BalanceBefore = balanceBefore;
        BalanceAfter = balanceAfter;
        Type = type;
        Description = description.Trim();
        OrderId = orderId;
        TransactionCode = transactionCode ?? GenerateCode(type);
    }

    private static string GenerateCode(WalletTransactionType type)
    {
        var prefix = type switch
        {
            WalletTransactionType.Deposit => "WTX_DEP",
            WalletTransactionType.Payment => "WTX_PAY",
            WalletTransactionType.Refund => "WTX_REF",
            _ => "WTX"
        };
        return $"{prefix}_{Clock.Now:yyyyMMdd}_{Guid.CreateVersion7().ToString("N")[..8].ToUpperInvariant()}";
    }
}
