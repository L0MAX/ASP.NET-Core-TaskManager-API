using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities;

/// <summary>
/// Project aggregate root — owns tasks and enforces invariants for its boundary.
/// </summary>
public sealed class Project : BaseEntity, IAggregateRoot
{
    private readonly List<ProjectTask> _tasks = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; } = null!;

    public IReadOnlyCollection<ProjectTask> Tasks => _tasks.AsReadOnly();

    private Project() { }

    internal static Project Create(Guid ownerId, string name, string? description)
    {
        if (ownerId == Guid.Empty)
            throw new DomainException("Project owner is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Project name is required.");

        if (name.Length > 200)
            throw new DomainException("Project name must not exceed 200 characters.");

        if (description?.Length > 2000)
            throw new DomainException("Project description must not exceed 2000 characters.");

        return new Project
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = name.Trim(),
            Description = description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    internal void AttachOwner(User owner)
    {
        Owner = owner;
        OwnerId = owner.Id;
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Project name is required.");

        if (name.Length > 200)
            throw new DomainException("Project name must not exceed 200 characters.");

        if (description?.Length > 2000)
            throw new DomainException("Project description must not exceed 2000 characters.");

        Name = name.Trim();
        Description = description?.Trim();
        SetUpdated();
    }

    public ProjectTask AddTask(
        string title,
        string? description,
        TaskPriority priority,
        DateTime? dueDate,
        Guid? assigneeId = null)
    {
        var task = ProjectTask.Create(Id, title, description, priority, dueDate, assigneeId);
        task.AttachToProject(this);
        _tasks.Add(task);
        SetUpdated();
        return task;
    }

    public ProjectTask GetTask(Guid taskId) =>
        _tasks.FirstOrDefault(t => t.Id == taskId)
        ?? throw new NotFoundException(nameof(ProjectTask), taskId);

    public void RemoveTask(Guid taskId)
    {
        var task = GetTask(taskId);
        _tasks.Remove(task);
        SetUpdated();
    }
}
