using TaskManager.Domain.Common;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;

    private RefreshToken() { }

    internal static RefreshToken Create(User user, string tokenHash, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new DomainException("Refresh token hash is required.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Refresh token expiration must be in the future.");

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Revoke(string? replacedByTokenHash = null)
    {
        if (RevokedAt.HasValue)
            return;

        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenHash = replacedByTokenHash;
        SetUpdated();
    }
}
