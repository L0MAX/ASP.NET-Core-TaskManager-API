using TaskManager.Domain.Common;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities;

/// <summary>
/// Comment entity owned by a <see cref="ProjectTask"/>.
/// </summary>
public sealed class Comment : BaseEntity
{
    public string Content { get; private set; } = string.Empty;

    public Guid TaskId { get; private set; }
    public ProjectTask Task { get; private set; } = null!;

    public Guid AuthorId { get; private set; }
    public User Author { get; private set; } = null!;

    private Comment() { }

    internal static Comment Create(Guid taskId, Guid authorId, string content)
    {
        if (taskId == Guid.Empty)
            throw new DomainException("Comment must belong to a task.");

        if (authorId == Guid.Empty)
            throw new DomainException("Comment author is required.");

        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Comment content is required.");

        if (content.Length > 4000)
            throw new DomainException("Comment must not exceed 4000 characters.");

        return new Comment
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            AuthorId = authorId,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Edit(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Comment content is required.");

        if (content.Length > 4000)
            throw new DomainException("Comment must not exceed 4000 characters.");

        Content = content.Trim();
        SetUpdated();
    }
}
