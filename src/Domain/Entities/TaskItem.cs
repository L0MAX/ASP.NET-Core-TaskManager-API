using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
