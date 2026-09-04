namespace Account.Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    string? CurrentUserId { get; }
}
