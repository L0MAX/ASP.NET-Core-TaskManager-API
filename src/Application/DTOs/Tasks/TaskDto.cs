using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Tasks;

public record TaskDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    TaskStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid? AssigneeId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
