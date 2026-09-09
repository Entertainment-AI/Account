using Account.Domain.Common;

namespace Account.Domain.Entities;

public class Plan : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int DurationMonths { get; private set; }
    public string? Description { get; private set; }
    public string? Features { get; private set; }
    public bool IsActive { get; private set; }

    private Plan() { } // EF Core

    private Plan(
        string name,
        string code,
        decimal price,
        int durationMonths,
        string? description = null,
        string? features = null,
        bool isActive = true)
    {
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Price = price < 0 ? throw new ArgumentException("Price cannot be negative.", nameof(price)) : price;
        DurationMonths = durationMonths <= 0 ? throw new ArgumentException("Duration must be at least 1 month.", nameof(durationMonths)) : durationMonths;
        Description = description?.Trim();
        Features = features?.Trim();
        IsActive = isActive;
    }

    public static Plan Create(
        string name,
        string code,
        decimal price,
        int durationMonths,
        string? description = null,
        string? features = null,
        bool isActive = true)
    {
        return new Plan(name, code, price, durationMonths, description, features, isActive);
    }

    public void UpdateDetails(string name, decimal price, int durationMonths, string? description, string? features)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name.Trim();
        if (price >= 0) Price = price;
        if (durationMonths > 0) DurationMonths = durationMonths;
        Description = description?.Trim();
        Features = features?.Trim();
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
