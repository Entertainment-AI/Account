namespace Account.Domain.Common.DateTimes;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
