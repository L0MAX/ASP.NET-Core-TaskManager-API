using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities;

/// <summary>
/// Task entity within the <see cref="Project"/> aggregate.
/// Named <c>ProjectTask</c> to avoid clashing with <see cref="System.Threading.Tasks.Task"/>.
/// </summary>
public sealed class ProjectTask : BaseEntity
{
    private readonly List<Comment> _comments = new();

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; } = TaskStatus.Pending;
    public TaskPriority Priority { get; private set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; private set; }

    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    public Guid? AssigneeId { get; private set; }
    public User? Assignee { get; private set; }

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private ProjectTask() { }

    internal static ProjectTask Create(
        Guid projectId,
        string title,
        string? description,
        TaskPriority priority,
        DateTime? dueDate,
        Guid? assigneeId)
    {
        if (projectId == Guid.Empty)
            throw new DomainException("Task must belong to a project.");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title is required.");

        if (title.Length > 200)
            throw new DomainException("Task title must not exceed 200 characters.");

        if (description?.Length > 2000)
            throw new DomainException("Task description must not exceed 2000 characters.");

        if (dueDate.HasValue && dueDate.Value <= DateTime.UtcNow)
            throw new DomainException("Due date must be in the future.");

        return new ProjectTask
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = title.Trim(),
            Description = description?.Trim(),
            Priority = priority,
            DueDate = dueDate,
            AssigneeId = assigneeId,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(
        string title,
        string? description,
        TaskStatus status,
        TaskPriority priority,
        DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title is required.");

        if (title.Length > 200)
            throw new DomainException("Task title must not exceed 200 characters.");

        if (description?.Length > 2000)
            throw new DomainException("Task description must not exceed 2000 characters.");

        if (dueDate.HasValue && dueDate.Value <= DateTime.UtcNow)
            throw new DomainException("Due date must be in the future.");

        Title = title.Trim();
        Description = description?.Trim();
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        SetUpdated();
    }

    public void AssignTo(Guid? assigneeId)
    {
        AssigneeId = assigneeId;
        SetUpdated();
    }

    public void ChangeStatus(TaskStatus status)
    {
        Status = status;
        SetUpdated();
    }

    internal void AttachToProject(Project project)
    {
        Project = project;
        ProjectId = project.Id;
    }

    public Comment AddComment(Guid authorId, string content)
    {
        var comment = Comment.Create(Id, authorId, content);
        _comments.Add(comment);
        SetUpdated();
        return comment;
    }
}
