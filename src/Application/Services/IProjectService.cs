using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Projects;

namespace TaskManager.Application.Services;

public interface IProjectService
{
    Task<PaginatedList<ProjectDto>> GetProjectsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProjectDto> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);
    Task<ProjectDto> UpdateProjectAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default);
    Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default);
}
