using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public TaskService(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TaskDto>> GetTasksAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        TaskStatus? status,
        TaskPriority? priority,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        await EnsureProjectAccessAsync(projectId, userId, cancellationToken);

        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        query = query.OrderByDescending(t => t.CreatedAt);

        var projected = query.ProjectTo<TaskDto>(_mapper.ConfigurationProvider);

        return await PaginatedList<TaskDto>.CreateAsync(projected, pageNumber, pageSize, cancellationToken);
    }

    public async Task<TaskDto> GetTaskByIdAsync(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        await EnsureProjectAccessAsync(projectId, userId, cancellationToken);

        var task = await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId, cancellationToken);

        if (task is null)
            throw new NotFoundException(nameof(ProjectTask), taskId);

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> CreateTaskAsync(
        CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.OwnerId == userId, cancellationToken);

        if (project is null)
            throw new NotFoundException(nameof(Project), request.ProjectId);

        var task = project.AddTask(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.AssigneeId);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> UpdateTaskAsync(
        Guid projectId,
        Guid taskId,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId, cancellationToken);

        if (project is null)
            throw new NotFoundException(nameof(Project), projectId);

        var task = project.GetTask(taskId);

        task.UpdateDetails(
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DueDate);

        task.AssignTo(request.AssigneeId);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(task);
    }

    public async Task DeleteTaskAsync(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId, cancellationToken);

        if (project is null)
            throw new NotFoundException(nameof(Project), projectId);

        project.RemoveTask(taskId);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureProjectAccessAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var exists = await _context.Projects
            .AnyAsync(p => p.Id == projectId && p.OwnerId == userId, cancellationToken);

        if (!exists)
            throw new NotFoundException(nameof(Project), projectId);
    }

    private Guid GetCurrentUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedException();

        return _currentUser.UserId.Value;
    }
}
