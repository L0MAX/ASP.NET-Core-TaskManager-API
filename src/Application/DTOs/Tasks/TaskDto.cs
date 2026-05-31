using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Tasks;

public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
