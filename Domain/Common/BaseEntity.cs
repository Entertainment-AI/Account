using Account.Domain.Common.DateTimes;

namespace Account.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public bool Deleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public string? DeletedBy { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = Clock.Now;
    }

    protected BaseEntity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.CreateVersion7() : id;
        CreatedAt = Clock.Now;
    }

    public void SetCreated(DateTime createdAt, string? createdBy)
    {
        CreatedAt = createdAt;
        CreatedBy = createdBy;
    }

    public void SetUpdated(DateTime updatedAt, string? updatedBy)
    {
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
    }

    public void SetDeleted(DateTime deletedAt, string? deletedBy)
    {
        Deleted = true;
        DeletedAt = deletedAt;
        DeletedBy = deletedBy;
    }

    public void Touch()
    {
        UpdatedAt = Clock.Now;
    }

    public void MarkAsDeleted()
    {
        Deleted = true;
        DeletedAt = Clock.Now;
        Touch();
    }
}
