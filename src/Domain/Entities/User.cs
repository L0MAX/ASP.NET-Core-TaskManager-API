using TaskManager.Domain.Common;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Domain.Entities;

/// <summary>
/// User aggregate root — identity, roles, refresh tokens, and owned projects.
/// </summary>
public sealed class User : BaseEntity, IAggregateRoot
{
    private readonly List<Project> _ownedProjects = new();
    private readonly List<UserRole> _userRoles = new();
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly List<ProjectTask> _assignedTasks = new();

    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    public IReadOnlyCollection<Project> OwnedProjects => _ownedProjects.AsReadOnly();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    public IReadOnlyCollection<ProjectTask> AssignedTasks => _assignedTasks.AsReadOnly();

    private User() { }

    public static User Register(Email email, string passwordHash, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public Project CreateProject(string name, string? description)
    {
        var project = Project.Create(Id, name, description);
        project.AttachOwner(this);
        _ownedProjects.Add(project);
        return project;
    }

    public void AssignRole(Role role)
    {
        if (_userRoles.Any(ur => ur.RoleId == role.Id))
            throw new ConflictException($"User already has role '{role.Name}'.");

        _userRoles.Add(UserRole.Create(this, role));
    }

    public RefreshToken IssueRefreshToken(string tokenHash, DateTime expiresAt)
    {
        var token = RefreshToken.Create(this, tokenHash, expiresAt);
        _refreshTokens.Add(token);
        return token;
    }

    public void RevokeRefreshToken(string tokenHash, string? replacedByTokenHash = null)
    {
        var token = _refreshTokens.FirstOrDefault(t => t.TokenHash == tokenHash)
            ?? throw new NotFoundException(nameof(RefreshToken), tokenHash);

        token.Revoke(replacedByTokenHash);
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        SetUpdated();
    }
}
