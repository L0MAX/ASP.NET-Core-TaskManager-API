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
        int pageNumber,
        int pageSize,
        TaskItemStatus? status,
        TaskPriority? priority,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        query = query.OrderByDescending(t => t.CreatedAt);

        var projected = query.ProjectTo<TaskDto>(_mapper.ConfigurationProvider);

        return await PaginatedList<TaskDto>.CreateAsync(projected, pageNumber, pageSize, cancellationToken);
    }

    public async Task<TaskDto> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var task = await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken);

        if (task is null)
            throw new NotFoundException(nameof(TaskItem), id);

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            Status = TaskItemStatus.Pending,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken);

        if (task is null)
            throw new NotFoundException(nameof(TaskItem), id);

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(task);
    }

    public async Task DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken);

        if (task is null)
            throw new NotFoundException(nameof(TaskItem), id);

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private Guid GetCurrentUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedException();

        return _currentUser.UserId.Value;
    }
}
