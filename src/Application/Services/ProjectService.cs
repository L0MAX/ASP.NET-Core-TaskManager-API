using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Projects;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public ProjectService(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProjectDto>> GetProjectsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var query = _context.Projects
            .AsNoTracking()
            .Where(p => p.OwnerId == userId)
            .OrderByDescending(p => p.CreatedAt);

        var projected = query.ProjectTo<ProjectDto>(_mapper.ConfigurationProvider);

        return await PaginatedList<ProjectDto>.CreateAsync(projected, pageNumber, pageSize, cancellationToken);
    }

    public async Task<ProjectDto> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var project = await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId, cancellationToken);

        if (project is null)
            throw new NotFoundException(nameof(Project), id);

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto> CreateProjectAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        var project = user.CreateProject(request.Name, request.Description);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto> UpdateProjectAsync(
        Guid id,
        UpdateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId, cancellationToken);

        if (project is null)
            throw new NotFoundException(nameof(Project), id);

        project.UpdateDetails(request.Name, request.Description);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId, cancellationToken);

        if (project is null)
            throw new NotFoundException(nameof(Project), id);

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private Guid GetCurrentUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedException();

        return _currentUser.UserId.Value;
    }
}
