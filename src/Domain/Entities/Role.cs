using TaskManager.Domain.Common;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities;

public sealed class Role : BaseEntity
{
    private readonly List<UserRole> _userRoles = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private Role() { }

    public static Role Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Role name is required.");

        if (name.Length > 50)
            throw new DomainException("Role name must not exceed 50 characters.");

        return new Role
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
