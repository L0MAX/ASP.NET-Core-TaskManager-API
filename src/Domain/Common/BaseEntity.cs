namespace TaskManager.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected void SetUpdated() => UpdatedAt = DateTime.UtcNow;

    internal void MarkCreated() => CreatedAt = DateTime.UtcNow;

    internal void MarkUpdated() => UpdatedAt = DateTime.UtcNow;
}
