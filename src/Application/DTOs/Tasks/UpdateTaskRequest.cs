using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Tasks;

public record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTime? DueDate);
