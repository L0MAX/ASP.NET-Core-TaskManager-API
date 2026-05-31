using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Tasks;

public record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid? AssigneeId = null);
