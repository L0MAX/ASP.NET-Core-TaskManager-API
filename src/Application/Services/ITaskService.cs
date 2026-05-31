using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Services;

public interface ITaskService
{
    Task<PaginatedList<TaskDto>> GetTasksAsync(
        int pageNumber,
        int pageSize,
        TaskItemStatus? status,
        TaskPriority? priority,
        CancellationToken cancellationToken = default);

    Task<TaskDto> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default);
}
