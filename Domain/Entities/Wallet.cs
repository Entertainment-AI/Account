using Account.Domain.Common;
using Account.Domain.Enums;

namespace Account.Domain.Entities;

public class Wallet : BaseEntity
{
    public Guid UserId { get; private set; }
    public decimal Balance { get; private set; }
    public string Currency { get; private set; } = "VND";
    public WalletStatus Status { get; private set; }

    private readonly List<WalletTransaction> _transactions = new();
    public IReadOnlyCollection<WalletTransaction> Transactions => _transactions.AsReadOnly();

    private Wallet() { } // EF Core

    private Wallet(Guid userId, string currency = "VND")
    {
        UserId = userId;
        Balance = 0;
        Currency = currency.Trim().ToUpperInvariant();
        Status = WalletStatus.Active;
    }

    public static Wallet Create(Guid userId, string currency = "VND")
    {
        return new Wallet(userId, currency);
    }

    public WalletTransaction Deposit(decimal amount, string description, Guid? orderId = null, string? transactionCode = null)
    {
        if (Status != WalletStatus.Active)
        {
            throw new InvalidOperationException("Wallet is not active.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Deposit amount must be greater than 0.", nameof(amount));
        }

        var before = Balance;
        Balance += amount;
        var after = Balance;

        var tx = new WalletTransaction(Id, amount, before, after, WalletTransactionType.Deposit, description, orderId, transactionCode);
        _transactions.Add(tx);
        Touch();
        return tx;
    }

    public WalletTransaction Deduct(decimal amount, string description, Guid? orderId = null, string? transactionCode = null)
    {
        if (Status != WalletStatus.Active)
        {
            throw new InvalidOperationException("Wallet is not active.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Deduction amount must be greater than 0.", nameof(amount));
        }

        if (Balance < amount)
        {
            throw new InvalidOperationException($"Insufficient balance. Current: {Balance:N0} {Currency}, Required: {amount:N0} {Currency}");
        }

        var before = Balance;
        Balance -= amount;
        var after = Balance;

        var tx = new WalletTransaction(Id, -amount, before, after, WalletTransactionType.Payment, description, orderId, transactionCode);
        _transactions.Add(tx);
        Touch();
        return tx;
    }

    public void Lock()
    {
        Status = WalletStatus.Locked;
        Touch();
    }

    public void Unlock()
    {
        Status = WalletStatus.Active;
        Touch();
    }
}
