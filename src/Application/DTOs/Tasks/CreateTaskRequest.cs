using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Tasks;

public record CreateTaskRequest(
    Guid ProjectId,
    string Title,
    string? Description,
    TaskPriority Priority = TaskPriority.Medium,
    DateTime? DueDate = null,
    Guid? AssigneeId = null);
