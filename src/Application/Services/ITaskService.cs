using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Services;

public interface ITaskService
{
    Task<PaginatedList<TaskDto>> GetTasksAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        TaskStatus? status,
        TaskPriority? priority,
        CancellationToken cancellationToken = default);

    Task<TaskDto> GetTaskByIdAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken = default);
    Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskDto> UpdateTaskAsync(Guid projectId, Guid taskId, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken = default);
}
