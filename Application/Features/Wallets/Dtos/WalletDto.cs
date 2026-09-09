namespace Account.Application.Features.Wallets.Dtos;

public record WalletDto(
    Guid Id,
    Guid UserId,
    decimal Balance,
    string Currency,
    string Status,
    IReadOnlyList<WalletTransactionDto> RecentTransactions
);

public record WalletTransactionDto(
    Guid Id,
    string TransactionCode,
    decimal Amount,
    decimal BalanceBefore,
    decimal BalanceAfter,
    string Type,
    string Description,
    Guid? OrderId,
    DateTime CreatedAt
);
